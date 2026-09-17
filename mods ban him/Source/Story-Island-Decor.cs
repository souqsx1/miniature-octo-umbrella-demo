using HarmonyLib;
using RaftModLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using HMLLibrary;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Object = UnityEngine.Object;

public partial class test_balloons : Mod
{
    private const int CustomCraftingCategoryId = 71;
    private const string CustomCraftingCategoryLabel = "Island Decorations";
    private const string SourceItemUniqueName = "Placeable_Streamer";


    private static EaselSocketProfile EaselSocketProfileState = new EaselSocketProfile(new Vector3(0.023f, 1.23f, 0.276f), new Vector3(-15f, 0f, 0f));
    private static readonly Dictionary<string, EaselSocketProfile> EaselSocketProfilesByItem = new Dictionary<string, EaselSocketProfile>
    {
        { Painting1ItemUniqueName, new EaselSocketProfile(new Vector3(0f, 1.449f, 0.12f), new Vector3(15f, 0f, 0f)) },
        { Painting2ItemUniqueName, new EaselSocketProfile(new Vector3(0f, 1.21f, 0.18f), new Vector3(15f, 0f, 0f)) },
        { Painting3ItemUniqueName, new EaselSocketProfile(new Vector3(0f, 1.069f, 0.21f), new Vector3(18f, 0f, 0f)) },
        { CanvasBlankItemUniqueName, new EaselSocketProfile(new Vector3(0f, 1.41f, 0.13f), new Vector3(15f, 0f, 0f)) }
    };
    private static readonly EaselSocketProfile GenericWallEaselSocketProfile = new EaselSocketProfile(new Vector3(0f, 1.41f, 0.13f), new Vector3(15f, 0f, 0f));
    private static SO_BlockQuadType easelPaintingSocketQuadType;
    private const int FirstLevelToPreload = 0;
    private const int LastLevelToPreload = 39;
    private const string LevelFilePrefix = "level";
    private const string CacheFolder = "Mods\\test-balloons-cache";
    private const int GeneratedIconSize = 256;
    private const int IconPreviewLayer = 30;
    private Sprite customCategoryIcon;

    private readonly List<Item_Base> registeredItems = new List<Item_Base>();
    private static test_balloons instance;
    public bool ExtraSettingsAPI_Loaded = false;
    public bool ExtraSettings_UseGenericWallEaselQuad = false;
    private bool previousUseGenericWallEaselQuad;
    private bool placementRegistered;
    private bool preloadStarted;
    private bool preloadCompleted;
    private Coroutine preloadRoutine;
    private static Transform prefabHolder;
    private Harmony harmony;
    private float nextPlacementRetryTime;
    private float nextVisualRefreshTime;
    private float nextGlobalIconRefreshTime;
    private const float PlacementRetryInterval = 1f;
    private const float VisualRefreshInterval = 2f;
    private const float GlobalIconRefreshInterval = 1f;

    public void Start()
    {
        Debug.Log("Mod test-balloons has been loaded!");
        instance = this;
        previousUseGenericWallEaselQuad = ExtraSettings_UseGenericWallEaselQuad;

        harmony = new Harmony("com.test_balloons.mod");
        harmony.PatchAll(typeof(test_balloons).Assembly);

        EnsurePrefabHolder();

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartMeshPreloadIfNeeded();
        TryRegisterItemForPlacement();
    }

    public override IEnumerable<(Sprite icon, CraftingCategory category)> CraftingCategories()
    {
        return new List<(Sprite icon, CraftingCategory category)>
        {
            (ResolveCustomCategoryIcon(), test_balloons_recipes.DecorCategory)
        };
    }

    private Sprite ResolveCustomCategoryIcon()
    {
        if (customCategoryIcon != null)
        {
            return customCategoryIcon;
        }

        Sprite embeddedIcon = TryLoadCustomCategoryIconFromEmbeddedFile();
        if (embeddedIcon != null)
        {
            customCategoryIcon = embeddedIcon;
            return customCategoryIcon;
        }

        Sprite base64Icon = TryLoadCustomCategoryIconFromBase64();
        if (base64Icon != null)
        {
            customCategoryIcon = base64Icon;
            return customCategoryIcon;
        }

        if (GeneratedIconsByUniqueName.TryGetValue(HedgeBush2ItemUniqueName, out Sprite largeHedgeIcon) && largeHedgeIcon != null)
        {
            customCategoryIcon = largeHedgeIcon;
            return customCategoryIcon;
        }

        if (GeneratedIconSprites.Count > 0 && GeneratedIconSprites[0] != null)
        {
            customCategoryIcon = GeneratedIconSprites[0];
            return customCategoryIcon;
        }

        return null;
    }

    private void Update()
    {
        if (previousUseGenericWallEaselQuad != ExtraSettings_UseGenericWallEaselQuad)
        {
            previousUseGenericWallEaselQuad = ExtraSettings_UseGenericWallEaselQuad;
            ApplyEaselSocketTransformToAllInstances();
        }

        
        if (!placementRegistered && Time.time >= nextPlacementRetryTime)
        {
            nextPlacementRetryTime = Time.time + PlacementRetryInterval;
            TryRegisterItemForPlacement();
        }


        if (placementRegistered && preloadCompleted && Time.time >= nextVisualRefreshTime)
        {
            nextVisualRefreshTime = Time.time + VisualRefreshInterval;
            RefreshPlacedBlockVisuals();
        }

        if (preloadCompleted && Time.time >= nextGlobalIconRefreshTime)
        {
            nextGlobalIconRefreshTime = Time.time + GlobalIconRefreshInterval;
            RefreshAllKnownItemIcons();
        }
    }

    private static void EnsurePrefabHolder()
    {
        if (prefabHolder != null)
        {
            return;
        }

        GameObject holder = new GameObject("test_balloons_prefabHolder");
        holder.SetActive(false);
        Object.DontDestroyOnLoad(holder);
        prefabHolder = holder.transform;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartMeshPreloadIfNeeded();
        TryRegisterItemForPlacement();
        RefreshAllKnownItemIcons();
    }

    private void StartMeshPreloadIfNeeded()
    {
        if (preloadStarted)
        {
            return;
        }

        preloadStarted = true;
        preloadRoutine = StartCoroutine(RunAllMeshPreloadsAsync());
    }

    private IEnumerator PreloadLevelMeshesAsync()
    {
        for (int levelIndex = FirstLevelToPreload; levelIndex <= LastLevelToPreload; levelIndex++)
        {
            string levelFile = LevelFilePrefix + levelIndex;

            bool loaded = false;
            yield return UseLevelAssetFile(
                levelFile,
                (file, manager) => file.file.GetAssetsOfType(AssetClassID.Transform)
                    .Where(transformInfo => manager.GetBaseField(file, transformInfo, AssetReadFlags.SkipMonoBehaviourFields)["m_Father.m_PathID"].AsLong == 0)
                    .Select(transformInfo => transformInfo.PathId),
                (bundle) =>
            {
                CacheMeshVisualsFromLoadedAssets();
                loaded = true;
            });

            if (!loaded)
            {
                Debug.LogWarning("Skipped level asset file " + levelFile + " while preloading mesh visuals.");
            }

            if (PreloadedVisuals.Count >= RequiredMeshNames.Count)
            {
                break;
            }
        }
    }

    private IEnumerator RunAllMeshPreloadsAsync()
    {
        yield return PreloadLevelMeshesAsync();
        yield return PreloadExplicitSourceMeshesAsync();

        preloadCompleted = true;
        RefreshRegisteredItemIcons();
        RefreshAllKnownItemIcons();
        RefreshRegisteredItemVisuals();
        RefreshPlacedBlockVisuals();
        Debug.Log("Mesh preload completed. " + PreloadedVisuals.Count + " by-name, " + PreloadedVisualsByIdentity.Count + " by-identity.");
        TryRegisterItemForPlacement();
    }

    private IEnumerator PreloadExplicitSourceMeshesAsync()
    {
        foreach (IGrouping<string, ModelDefinition> group in ExplicitSourceModelsByAssetFile)
        {
            string assetFile = group.Key;
            List<long> wantedPathIds = group.Select(model => model.SourcePathId).Distinct().ToList();

            bool loaded = false;
            yield return UseLevelAssetFile(
                assetFile,
                (file, manager) => wantedPathIds.Concat(
                    file.file.GetAssetsOfType(AssetClassID.Transform)
                        .Where(transformInfo => manager.GetBaseField(file, transformInfo, AssetReadFlags.SkipMonoBehaviourFields)["m_Father.m_PathID"].AsLong == 0)
                        .Select(transformInfo => transformInfo.PathId)),
                (bundle) =>
            {
                CachePreciseMeshesFromLoadedAssets(assetFile, wantedPathIds, bundle);
                loaded = true;
            },
            ExplicitSourceCacheTag);

            if (!loaded)
            {
                Debug.LogWarning("Failed to load explicit-source meshes from " + assetFile + ".");
            }
        }
    }

    private static void CachePreciseMeshesFromLoadedAssets(string assetFile, List<long> wantedPathIds, AssetBundle bundle)
    {
        if (bundle == null || wantedPathIds == null)
        {
            return;
        }

        MeshFilter[] meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();

        for (int i = 0; i < wantedPathIds.Count; i++)
        {
            long pathId = wantedPathIds[i];
            MeshIdentity identity = new MeshIdentity(assetFile, pathId);
            if (PreloadedVisualsByIdentity.ContainsKey(identity))
            {
                continue;
            }

            Mesh preciseMesh = bundle.LoadAsset<Mesh>("asset" + i);
            if (preciseMesh == null)
            {
                Debug.LogWarning("Could not load mesh at path ID " + pathId + " from " + assetFile + ".");
                continue;
            }

            Renderer matchedRenderer = null;
            for (int filterIndex = 0; filterIndex < meshFilters.Length; filterIndex++)
            {
                MeshFilter meshFilter = meshFilters[filterIndex];
                if (meshFilter != null && ReferenceEquals(meshFilter.sharedMesh, preciseMesh))
                {
                    matchedRenderer = meshFilter.GetComponent<Renderer>();
                    break;
                }
            }

            if (matchedRenderer == null || matchedRenderer.sharedMaterials == null || matchedRenderer.sharedMaterials.Length == 0)
            {
                Debug.LogWarning("Loaded mesh at path ID " + pathId + " from " + assetFile +
                    " (\"" + preciseMesh.name + "\") but couldn't find a renderer using it with materials.");
                continue;
            }

            PreloadedVisualsByIdentity[identity] = new MeshVisualData(preciseMesh, matchedRenderer.sharedMaterials, null);
        }
    }

    private IEnumerator UseLevelAssetFile(string assetFile, Func<AssetsFileInstance, AssetsManager, IEnumerable<long>> selectAssets, Action<AssetBundle> onComplete, string cacheTag = "")
    {
        AssetBundleCreateRequest request = null;
        try
        {
            string targetPath = GetTargetPath(assetFile, cacheTag);
            string loaderPath = GetLoaderPath(assetFile, cacheTag);
            string raftDataPath = Path.Combine("Raft_Data", assetFile);
            if (File.Exists(targetPath) && File.Exists(loaderPath) && File.Exists(raftDataPath))
            {
                string[] lines = File.ReadAllLines(targetPath);
                long time;
                if (lines != null && lines.Length == 2 && lines[0] == assetFile && long.TryParse(lines[1], out time) && time == File.GetLastWriteTimeUtc(raftDataPath).Ticks)
                {
                    request = AssetBundle.LoadFromFileAsync(loaderPath);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning("An exception occurred trying to fetch the level cache for " + assetFile + "\n" + exception);
        }

        if (request != null)
        {
            yield return request;
            if (request.assetBundle != null)
            {
                AssetBundleRequest sceneRequest = request.assetBundle.LoadAllAssetsAsync();
                yield return sceneRequest;
                _ = sceneRequest.allAssets;
                try
                {
                    onComplete(request.assetBundle);
                }
                finally
                {
                    request.assetBundle.Unload(true);
                }
                yield break;
            }
        }

        Task task = CreateLoaderAsync(assetFile, selectAssets, cacheTag);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.LogWarning("Failed generating cache loader for " + assetFile + "\n" + task.Exception);
            yield break;
        }

        request = AssetBundle.LoadFromFileAsync(GetLoaderPath(assetFile, cacheTag));
        yield return request;
        if (request.assetBundle == null)
        {
            yield break;
        }

        AssetBundleRequest loadRequest = request.assetBundle.LoadAllAssetsAsync();
        yield return loadRequest;
        _ = loadRequest.allAssets;

        try
        {
            onComplete(request.assetBundle);
        }
        finally
        {
            request.assetBundle.Unload(false);
        }
    }

    private Task CreateLoaderAsync(string assetsFile, Func<AssetsFileInstance, AssetsManager, IEnumerable<long>> selectAssets, string cacheTag)
    {
        return Task.Run(() => CreateLoader(assetsFile, selectAssets, cacheTag));
    }

    private void CreateLoader(string assetsFile, Func<AssetsFileInstance, AssetsManager, IEnumerable<long>> selectAssets, string cacheTag)
    {
        try
        {
            AssetsManager manager = new AssetsManager();
            byte[] lz4Data = TryLoadEmbeddedFileBytes("Dependencies/lz4.tpk");
            if (lz4Data == null || lz4Data.Length == 0)
            {
                throw new FileNotFoundException("Could not locate embedded lz4.tpk resource.");
            }

            using (MemoryStream data = new MemoryStream(lz4Data))
            {
                manager.LoadClassPackage(data);
            }

            AssetsFileInstance assetsFileInstance;
            IEnumerable<long> assets;
            string raftDataPath = Path.Combine("Raft_Data", assetsFile);
            using (FileStream sceneFile = File.OpenRead(raftDataPath))
            {
                assetsFileInstance = manager.LoadAssetsFile(sceneFile, false);
                manager.LoadClassDatabaseFromPackage(assetsFileInstance.file.Metadata.UnityVersion);
                assets = selectAssets(assetsFileInstance, manager).ToList();
                manager.UnloadAllAssetsFiles();
            }

            BundleFileInstance bundleFile;
            byte[] templateBundleData = TryLoadEmbeddedFileBytes("Dependencies/templatebundle");
            if (templateBundleData == null || templateBundleData.Length == 0)
            {
                throw new FileNotFoundException("Could not locate embedded templatebundle resource.");
            }

            using (MemoryStream templateBundle = new MemoryStream(templateBundleData))
            {
                string bundleName = assetsFile + "loader" + UnityEngine.Random.Range(0, ushort.MaxValue + 1).ToString("X4");
                bundleFile = manager.LoadBundleFile(templateBundle, bundleName);
                bundleFile.file.BlockAndDirInfo.DirectoryInfos[0].Name = bundleName;

                assetsFileInstance = manager.LoadAssetsFileFromBundle(bundleFile, 0, false);
                assetsFileInstance.file.Metadata.Externals[0].PathName = assetsFile;

                AssetFileInfo bundleAsset = assetsFileInstance.file.GetAssetsOfType(AssetClassID.AssetBundle)[0];
                AssetTypeValueField bundleField = manager.GetBaseField(assetsFileInstance, bundleAsset, AssetReadFlags.SkipMonoBehaviourFields);
                bundleField["m_AssetBundleName"].AsString = bundleField["m_Name"].AsString = bundleName;

                int index = 0;
                foreach (long pathId in assets)
                {
                    if (index == bundleField["m_Container.Array"].Children.Count)
                    {
                        bundleField["m_Container.Array"].Children.Add(ValueBuilder.DefaultValueFieldFromArrayTemplate(bundleField["m_Container.Array"]));
                        bundleField["m_PreloadTable.Array"].Children.Add(ValueBuilder.DefaultValueFieldFromArrayTemplate(bundleField["m_PreloadTable.Array"]));
                    }

                    bundleField["m_Container.Array"][index]["first"].AsString = "asset" + index;
                    bundleField["m_Container.Array"][index]["second.preloadIndex"].AsInt = index;
                    bundleField["m_Container.Array"][index]["second.preloadSize"].AsInt = 1;
                    bundleField["m_Container.Array"][index]["second.asset.m_FileID"].AsInt = 1;
                    bundleField["m_Container.Array"][index]["second.asset.m_PathID"].AsLong = pathId;
                    bundleField["m_PreloadTable.Array"][index]["m_FileID"].AsInt = 1;
                    bundleField["m_PreloadTable.Array"][index]["m_PathID"].AsLong = pathId;
                    index++;
                }

                bundleAsset.SetNewData(bundleField);
                bundleFile.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(assetsFileInstance.file);

                if (!Directory.Exists(CacheFolder))
                {
                    Directory.CreateDirectory(CacheFolder);
                }

                string loaderPath = GetLoaderPath(assetsFile, cacheTag);
                string targetPath = GetTargetPath(assetsFile, cacheTag);

                if (File.Exists(loaderPath))
                {
                    File.Delete(loaderPath);
                }

                File.WriteAllLines(targetPath, new[]
                {
                    assetsFile,
                    File.GetLastWriteTimeUtc(raftDataPath).Ticks.ToString()
                });

                using (FileStream file = File.Open(loaderPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (AssetsFileWriter writer = new AssetsFileWriter(file))
                {
                    bundleFile.file.Write(writer);
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    private byte[] TryLoadEmbeddedFileBytes(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        foreach (string candidate in GetEmbeddedResourceNameCandidates(fileName))
        {
            try
            {
                byte[] data = GetEmbeddedFileBytes(candidate);
                if (data != null && data.Length > 0)
                {
                    return data;
                }
            }
            catch (Exception)
            {
            }
        }

        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (string manifestName in assembly.GetManifestResourceNames())
            {
                if (manifestName.Equals(fileName, StringComparison.OrdinalIgnoreCase) ||
                    manifestName.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase))
                {
                    using (Stream stream = assembly.GetManifestResourceStream(manifestName))
                    {
                        if (stream == null)
                        {
                            continue;
                        }

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }

        return null;
    }

    private IEnumerable<string> GetEmbeddedResourceNameCandidates(string fileName)
    {
        string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        string rootNamespace = GetType().Namespace;
        List<string> candidates = new List<string> { fileName };

        if (!string.IsNullOrEmpty(assemblyName))
        {
            candidates.Add(assemblyName + "." + fileName);
        }

        if (!string.IsNullOrEmpty(rootNamespace))
        {
            candidates.Add(rootNamespace + "." + fileName);
        }

        if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(rootNamespace))
        {
            candidates.Add(rootNamespace + "." + assemblyName + "." + fileName);
        }

        return candidates.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static string GetLoaderPath(string assetFile, string cacheTag = "")
    {
        return Path.Combine(CacheFolder, assetFile + cacheTag + ".loader");
    }

    private static string GetTargetPath(string assetFile, string cacheTag = "")
    {
        return Path.Combine(CacheFolder, assetFile + cacheTag + ".target");
    }

    private static void CacheMeshVisualsFromLoadedAssets()
    {
        MeshFilter[] meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();
        for (int index = 0; index < meshFilters.Length; index++)
        {
            MeshFilter meshFilter = meshFilters[index];
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                continue;
            }

            Mesh sharedMesh = meshFilter.sharedMesh;
            string meshName = sharedMesh.name;
            if (!RequiredMeshNames.Contains(meshName))
            {
                continue;
            }

            Renderer renderer = meshFilter.GetComponent<Renderer>();
            if (renderer == null || renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
            {
                continue;
            }

            if (CompositeMeshNames.Contains(meshName))
            {
                MeshVisualData earlyExitData;
                if (PreloadedVisuals.TryGetValue(meshName, out earlyExitData) && earlyExitData != null
                    && CountTemplateRenderers(earlyExitData.CompositeTemplate) >= 2)
                {
                    continue;
                }
            }

            GameObject compositeTemplate = BuildCompositeTemplateIfNeeded(meshName, meshFilter.transform);
            MeshVisualData existingData;
            if (PreloadedVisuals.TryGetValue(meshName, out existingData) && existingData != null)
            {
                if (CompositeMeshNames.Contains(meshName))
                {
                    int existingRendererCount = CountTemplateRenderers(existingData.CompositeTemplate);
                    int candidateRendererCount = CountTemplateRenderers(compositeTemplate);
                    if (candidateRendererCount > existingRendererCount)
                    {
                        PreloadedVisuals[meshName] = new MeshVisualData(sharedMesh, renderer.sharedMaterials, compositeTemplate);
                    }
                }

                continue;
            }

            PreloadedVisuals[meshName] = new MeshVisualData(sharedMesh, renderer.sharedMaterials, compositeTemplate);
            if (PreloadedVisuals.Count >= RequiredMeshNames.Count)
            {
                return;
            }
        }
    }

    private static int CountTemplateRenderers(GameObject template)
    {
        if (template == null)
        {
            return 0;
        }

        MeshRenderer[] renderers = template.GetComponentsInChildren<MeshRenderer>(true);
        return renderers != null ? renderers.Length : 0;
    }

    private static GameObject BuildCompositeTemplateIfNeeded(string meshName, Transform sourceMeshTransform)
    {
        if (!CompositeMeshNames.Contains(meshName) || sourceMeshTransform == null)
        {
            return null;
        }

        Transform compositeRoot = FindCompositeRootForMesh(sourceMeshTransform);
        if (compositeRoot == null)
        {
            return null;
        }

        GameObject template = new GameObject(meshName + "_CompositeTemplate");
        template.name = meshName + "_CompositeTemplate";
        template.hideFlags = HideFlags.HideAndDontSave;
        BuildRenderableSnapshot(compositeRoot, sourceMeshTransform, template.transform);
        AppendNearbyCompositeRenderersIfNeeded(meshName, sourceMeshTransform, template.transform);
        if (template.transform.childCount == 0)
        {
            Object.DestroyImmediate(template);
            return null;
        }

        template.SetActive(false);

        if (prefabHolder != null)
        {
            template.transform.SetParent(prefabHolder, false);
        }
        else
        {
            Object.DontDestroyOnLoad(template);
        }

        return template;
    }

    private static Transform FindCompositeRootForMesh(Transform sourceMeshTransform)
    {
        if (sourceMeshTransform == null)
        {
            return null;
        }

        MeshFilter selfFilter = sourceMeshTransform.GetComponent<MeshFilter>();
        if (selfFilter != null && selfFilter.sharedMesh != null && selfFilter.sharedMesh.name == RtCameraBaseMeshName)
        {
            return sourceMeshTransform;
        }

        Transform current = sourceMeshTransform;
        Transform bestRoot = null;
        int bestRendererCount = 0;
        for (int depth = 0; depth < 12; depth++)
        {
            int rendererCount = CountMeshRenderers(current);
            if (rendererCount >= 2 && rendererCount <= 256 && rendererCount > bestRendererCount)
            {
                bestRendererCount = rendererCount;
                bestRoot = current;
            }

            current = current.parent;
            if (current == null)
            {
                break;
            }
        }

        return bestRoot;
    }

    private static void BuildRenderableSnapshot(Transform snapshotRoot, Transform anchorTransform, Transform targetParent)
    {
        if (snapshotRoot == null || anchorTransform == null || targetParent == null)
        {
            return;
        }

        MeshRenderer[] renderers = snapshotRoot.GetComponentsInChildren<MeshRenderer>(true);
        for (int index = 0; index < renderers.Length; index++)
        {
            MeshRenderer sourceRenderer = renderers[index];
            if (sourceRenderer == null)
            {
                continue;
            }

            MeshFilter sourceFilter = sourceRenderer.GetComponent<MeshFilter>();
            if (sourceFilter == null || sourceFilter.sharedMesh == null || sourceRenderer.sharedMaterials == null || sourceRenderer.sharedMaterials.Length == 0)
            {
                continue;
            }

            AddSnapshotRendererIfMissing(sourceRenderer, anchorTransform, targetParent);
        }
    }

    private static void AppendNearbyCompositeRenderersIfNeeded(string meshName, Transform anchorTransform, Transform targetParent)
    {
        if (targetParent == null || anchorTransform == null || targetParent.childCount >= 2)
        {
            return;
        }

        if (meshName != TangaroaKitchenIslandWineShelfMeshName && meshName != TangaroaKitchenFridgeMeshName
            && meshName != RtCameraBaseMeshName)
        {
            return;
        }

        // Security camera: find RT_Camera
        if (meshName == RtCameraBaseMeshName)
        {
            AppendExplicitCompanionMesh(anchorTransform, targetParent, "RT_Camera", 2.5f);
            return;
        }

        MeshRenderer anchorRenderer = anchorTransform.GetComponent<MeshRenderer>();
        if (anchorRenderer == null || anchorRenderer.sharedMaterials == null || anchorRenderer.sharedMaterials.Length == 0)
        {
            return;
        }

        Bounds anchorBounds = anchorRenderer.bounds;
        float maxDistance = meshName == TangaroaKitchenIslandWineShelfMeshName ? 1.8f : 2.2f;

        MeshRenderer[] allRenderers = Resources.FindObjectsOfTypeAll<MeshRenderer>();
        for (int index = 0; index < allRenderers.Length; index++)
        {
            MeshRenderer candidate = allRenderers[index];
            if (candidate == null || candidate.transform == anchorTransform)
            {
                continue;
            }

            MeshFilter candidateFilter = candidate.GetComponent<MeshFilter>();
            if (candidateFilter == null || candidateFilter.sharedMesh == null || candidate.sharedMaterials == null || candidate.sharedMaterials.Length == 0)
            {
                continue;
            }

            if (meshName == TangaroaKitchenIslandWineShelfMeshName && candidateFilter.sharedMesh.name == TangaroaKitchenCuttingBoardMeshName)
            {
                continue;
            }

            if (Vector3.Distance(candidate.bounds.center, anchorBounds.center) > maxDistance)
            {
                continue;
            }

            if (!BoundsNearOrIntersect(anchorBounds, candidate.bounds, 0.25f))
            {
                continue;
            }

            if (!ShareAnyMaterial(anchorRenderer.sharedMaterials, candidate.sharedMaterials))
            {
                continue;
            }

            AddSnapshotRendererIfMissing(candidate, anchorTransform, targetParent);
        }
    }

    private static void AppendExplicitCompanionMesh(Transform anchorTransform, Transform targetParent, string companionMeshName, float maxDistance)
    {
        if (anchorTransform == null || targetParent == null || string.IsNullOrEmpty(companionMeshName))
        {
            return;
        }

        MeshFilter[] allFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();
        for (int i = 0; i < allFilters.Length; i++)
        {
            MeshFilter filter = allFilters[i];
            if (filter == null || filter.sharedMesh == null || filter.sharedMesh.name != companionMeshName)
            {
                continue;
            }

            if (Vector3.Distance(filter.transform.position, anchorTransform.position) > maxDistance)
            {
                continue;
            }

            MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
            if (renderer == null || renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
            {
                continue;
            }

            GameObject companionChild = AddSnapshotRendererIfMissing(renderer, anchorTransform, targetParent);
            CopyAnimatorIfPresent(filter.transform, companionChild);
        }
    }

    private static GameObject AddSnapshotRendererIfMissing(MeshRenderer sourceRenderer, Transform anchorTransform, Transform targetParent)
    {
        if (sourceRenderer == null || anchorTransform == null || targetParent == null)
        {
            return null;
        }

        MeshFilter sourceFilter = sourceRenderer.GetComponent<MeshFilter>();
        if (sourceFilter == null || sourceFilter.sharedMesh == null || sourceRenderer.sharedMaterials == null || sourceRenderer.sharedMaterials.Length == 0)
        {
            return null;
        }

        Vector3 localPosition = anchorTransform.InverseTransformPoint(sourceRenderer.transform.position);
        Quaternion localRotation = Quaternion.Inverse(anchorTransform.rotation) * sourceRenderer.transform.rotation;
        Vector3 localScale = DivideVectors(sourceRenderer.transform.lossyScale, anchorTransform.lossyScale);

        Transform existingMatch;
        if (HasMatchingSnapshotEntry(targetParent, sourceFilter.sharedMesh, localPosition, out existingMatch))
        {
            return existingMatch != null ? existingMatch.gameObject : null;
        }

        GameObject child = new GameObject(sourceRenderer.name);
        child.transform.SetParent(targetParent, false);
        child.transform.localPosition = localPosition;
        child.transform.localRotation = localRotation;
        child.transform.localScale = localScale;

        MeshFilter childFilter = child.AddComponent<MeshFilter>();
        childFilter.sharedMesh = sourceFilter.sharedMesh;

        MeshRenderer childRenderer = child.AddComponent<MeshRenderer>();
        childRenderer.sharedMaterials = sourceRenderer.sharedMaterials;
        childRenderer.shadowCastingMode = sourceRenderer.shadowCastingMode;
        childRenderer.receiveShadows = sourceRenderer.receiveShadows;
        childRenderer.lightProbeUsage = sourceRenderer.lightProbeUsage;
        childRenderer.reflectionProbeUsage = sourceRenderer.reflectionProbeUsage;
        childRenderer.enabled = true;

        return child;
    }

    private static void CopyAnimatorIfPresent(Transform sourceTransform, GameObject targetObject)
    {
        if (sourceTransform == null || targetObject == null || targetObject.GetComponent<Animator>() != null)
        {
            return;
        }

        Animator sourceAnimator = sourceTransform.GetComponent<Animator>();
        if (sourceAnimator == null && sourceTransform.parent != null)
        {
            sourceAnimator = sourceTransform.parent.GetComponent<Animator>();
        }

        if (sourceAnimator == null || sourceAnimator.runtimeAnimatorController == null)
        {
            return;
        }

        Animator targetAnimator = targetObject.AddComponent<Animator>();
        targetAnimator.runtimeAnimatorController = sourceAnimator.runtimeAnimatorController;
        targetAnimator.avatar = sourceAnimator.avatar;
        targetAnimator.applyRootMotion = false;
        targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    private static bool HasMatchingSnapshotEntry(Transform targetParent, Mesh mesh, Vector3 localPosition, out Transform matchedChild)
    {
        matchedChild = null;

        if (targetParent == null || mesh == null)
        {
            return false;
        }

        for (int index = 0; index < targetParent.childCount; index++)
        {
            Transform child = targetParent.GetChild(index);
            if (child == null)
            {
                continue;
            }

            MeshFilter childFilter = child.GetComponent<MeshFilter>();
            if (childFilter == null || childFilter.sharedMesh != mesh)
            {
                continue;
            }

            if ((child.localPosition - localPosition).sqrMagnitude <= 0.0004f)
            {
                matchedChild = child;
                return true;
            }
        }

        return false;
    }

    private static bool BoundsNearOrIntersect(Bounds first, Bounds second, float padding)
    {
        Bounds expanded = first;
        expanded.Expand(padding * 2f);
        return expanded.Intersects(second);
    }

    private static bool ShareAnyMaterial(Material[] firstMaterials, Material[] secondMaterials)
    {
        if (firstMaterials == null || secondMaterials == null)
        {
            return false;
        }

        for (int firstIndex = 0; firstIndex < firstMaterials.Length; firstIndex++)
        {
            Material first = firstMaterials[firstIndex];
            if (first == null)
            {
                continue;
            }

            for (int secondIndex = 0; secondIndex < secondMaterials.Length; secondIndex++)
            {
                Material second = secondMaterials[secondIndex];
                if (second == null)
                {
                    continue;
                }

                if (ReferenceEquals(first, second) || first.name == second.name)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static Vector3 DivideVectors(Vector3 numerator, Vector3 denominator)
    {
        return new Vector3(
            Mathf.Abs(denominator.x) > 0.0001f ? numerator.x / denominator.x : numerator.x,
            Mathf.Abs(denominator.y) > 0.0001f ? numerator.y / denominator.y : numerator.y,
            Mathf.Abs(denominator.z) > 0.0001f ? numerator.z / denominator.z : numerator.z);
    }

    private static int CountMeshRenderers(Transform root)
    {
        if (root == null)
        {
            return 0;
        }

        MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        return renderers != null ? renderers.Length : 0;
    }

    private void TryRegisterItemForPlacement()
    {
        if (placementRegistered)
        {
            return;
        }

        if (!ItemManager.ItemsLoaded)
        {
            Debug.LogWarning("ItemManager has not finished loading yet. Placement registration will be retried after the next scene load.");
            return;
        }

        registeredItems.Clear();
        int recipeLine = 1;
        int recipeColumn = 1;

        for (int index = 0; index < ModelDefinitions.Length; index++)
        {
            ModelDefinition model = ModelDefinitions[index];
            Item_Base item = CreateCustomWallItem(model);
            if (item == null)
            {
                Debug.LogWarning("Could not create item " + model.UniqueName + ". Check source template and mesh availability.");
                continue;
            }

            ApplyRecipeMenuLine(item, recipeLine);
            if (recipeColumn == 4)
            {
                recipeColumn = 1;
                recipeLine++;
            }
            else
            {
                recipeColumn++;
            }

            RAPI.RegisterItem(item);
            for (int quadIndex = 0; quadIndex < model.QuadTypes.Length; quadIndex++)
            {
                RAPI.AddItemToBlockQuadType(item, model.QuadTypes[quadIndex]);
            }

            registeredItems.Add(item);
        }

        if (registeredItems.Count == 0)
        {
            Debug.LogWarning("No custom model items could be registered.");
            return;
        }

        placementRegistered = true;

        if (preloadCompleted)
        {
            RefreshRegisteredItemIcons();
            RefreshRegisteredItemVisuals();
            RefreshPlacedBlockVisuals();
        }

        Debug.Log("Registered " + registeredItems.Count + " custom prefab items.");
    }

    private void RefreshRegisteredItemVisuals()
    {
        for (int itemIndex = 0; itemIndex < registeredItems.Count; itemIndex++)
        {
            Item_Base item = registeredItems[itemIndex];
            if (item == null || item.settings_buildable == null)
            {
                continue;
            }

            ModelDefinition model;
            if (!ModelsByUniqueName.TryGetValue(item.UniqueName, out model))
            {
                continue;
            }

            Block[] blockPrefabs = item.settings_buildable.GetBlockPrefabs();
            for (int blockIndex = 0; blockIndex < blockPrefabs.Length; blockIndex++)
            {
                Block blockPrefab = blockPrefabs[blockIndex];
                if (blockPrefab == null)
                {
                    continue;
                }

                ApplyCustomModelIfAvailable(blockPrefab.gameObject, model);
                blockPrefab.buildableItem = item;
                blockPrefab.itemToReturnOnDestroy = item;
            }
        }
    }

    private void RefreshRegisteredItemIcons()
    {
        for (int itemIndex = 0; itemIndex < registeredItems.Count; itemIndex++)
        {
            Item_Base item = registeredItems[itemIndex];
            if (item == null)
            {
                continue;
            }

            ModelDefinition model;
            if (!ModelsByUniqueName.TryGetValue(item.UniqueName, out model))
            {
                continue;
            }

            AssignGeneratedIcon(item, model);
        }
    }

    private static void RefreshAllKnownItemIcons()
    {
        for (int modelIndex = 0; modelIndex < ModelDefinitions.Length; modelIndex++)
        {
            ModelDefinition model = ModelDefinitions[modelIndex];
            Item_Base item = ItemManager.GetItemByName(model.UniqueName);
            if (item == null)
            {
                continue;
            }

            AssignGeneratedIcon(item, model);
        }
    }

    private static void RefreshPlacedBlockVisuals()
    {
        List<Block> placedBlocks = BlockCreator.GetPlacedBlocks();
        for (int blockIndex = 0; blockIndex < placedBlocks.Count; blockIndex++)
        {
            Block placedBlock = placedBlocks[blockIndex];
            if (placedBlock == null || placedBlock.buildableItem == null)
            {
                continue;
            }

            if (placedBlock.gameObject.GetComponent<ConfiguredColliders>() != null)
            {
                continue;
            }

            ModelDefinition model;
            if (!ModelsByUniqueName.TryGetValue(placedBlock.buildableItem.UniqueName, out model))
            {
                continue;
            }

            ApplyCustomModelIfAvailable(placedBlock.gameObject, model);

            if (model.UniqueName == Easel1ItemUniqueName)
            {
                Transform socketTransform = placedBlock.transform.Find("CustomVisual/EaselPaintingSocket");
                if (socketTransform == null)
                {
                    socketTransform = placedBlock.transform.Find("EaselPaintingSocket");
                }

                if (socketTransform != null)
                {
 //                   Debug.Log("[SID-EaselSocketDebug] Easel placed | root=" + placedBlock.transform.position +
  //                            " | socketLocalPos=" + socketTransform.localPosition +
   //                           " | socketLocalRot=" + socketTransform.localEulerAngles +
    //                          " | socketWorldPos=" + socketTransform.position +
   //                           " | socketWorldRot=" + socketTransform.eulerAngles);
                }
                else
                {
                    Debug.LogWarning("[SID-EaselSocketDebug] Easel placed but socket not found on " + placedBlock.gameObject.name);
                }
            }

            if (IsEaselSocketItem(model.UniqueName))
            {
                Transform visualTransform = placedBlock.transform.Find("CustomVisual");
//                Debug.Log("[SID-EaselSocketDebug] Painting/Canvas placed | item=" + model.UniqueName +
 //                         " | rootPos=" + placedBlock.transform.position +
 //                         " | rootRot=" + placedBlock.transform.eulerAngles +
  //                        " | visualLocalPos=" + (visualTransform != null ? visualTransform.localPosition.ToString() : "<none>") +
  //                        " | visualLocalRot=" + (visualTransform != null ? visualTransform.localEulerAngles.ToString() : "<none>") +
   //                       " | visualWorldPos=" + (visualTransform != null ? visualTransform.position.ToString() : "<none>") +
   //                       " | visualWorldRot=" + (visualTransform != null ? visualTransform.eulerAngles.ToString() : "<none>"));
            }

            placedBlock.gameObject.AddComponent<ConfiguredColliders>();
        }
    }

    private static bool IsEaselSocketItem(string uniqueName)
    {
        if (string.IsNullOrEmpty(uniqueName))
        {
            return false;
        }

        for (int index = 0; index < EaselSocketAcceptedUniqueNames.Length; index++)
        {
            if (EaselSocketAcceptedUniqueNames[index] == uniqueName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsWallQuadPlaceableItem(string uniqueName)
    {
        if (string.IsNullOrEmpty(uniqueName) || !ModelsByUniqueName.TryGetValue(uniqueName, out ModelDefinition model) || model.QuadTypes == null)
        {
            return false;
        }

        for (int index = 0; index < model.QuadTypes.Length; index++)
        {
            if (model.QuadTypes[index] == RBlockQuadType.quad_wall)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasWallQuadPlacementSupport(Item_Base item)
    {
        if (item == null || item.settings_buildable == null)
        {
            return false;
        }

        Block[] blockPrefabs = item.settings_buildable.GetBlockPrefabs();
        if (blockPrefabs == null)
        {
            return false;
        }

        for (int blockIndex = 0; blockIndex < blockPrefabs.Length; blockIndex++)
        {
            Block blockPrefab = blockPrefabs[blockIndex];
            if (blockPrefab == null)
            {
                continue;
            }

            BlockQuad[] quads = blockPrefab.GetComponentsInChildren<BlockQuad>(true);
            for (int quadIndex = 0; quadIndex < quads.Length; quadIndex++)
            {
                BlockQuad quad = quads[quadIndex];
                if (quad == null)
                {
                    continue;
                }

                if (quad.acceptableBuildSides != null)
                {
                    for (int sideIndex = 0; sideIndex < quad.acceptableBuildSides.Length; sideIndex++)
                    {
                        BlockSurface surface = quad.acceptableBuildSides[sideIndex];
                        if (surface.dpsType == DPS.Wall)
                        {
                            return true;
                        }
                    }
                }

                if (quad.quadType != null)
                {
                    string quadTypeName = quad.quadType.name;
                    if (quadTypeName == "Quad_Wall" || quadTypeName.IndexOf("wall", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool CanUseEaselSocketRedirect(Item_Base selectedBuildableItem)
    {
        if (selectedBuildableItem == null)
        {
            return false;
        }

        if (IsEaselSocketItem(selectedBuildableItem.UniqueName))
        {
            return true;
        }

        if (!IsGenericWallEaselQuadEnabled())
        {
            return false;
        }

        if (IsWallQuadPlaceableItem(selectedBuildableItem.UniqueName))
        {
            return true;
        }

        return HasWallQuadPlacementSupport(selectedBuildableItem);
    }

    private static Item_Base CreateCustomWallItem(ModelDefinition model)
    {
        Item_Base sourceItem = ItemManager.GetItemByName(SourceItemUniqueName);
        if (sourceItem == null)
        {
            return null;
        }

        Item_Base customItem = ScriptableObject.Instantiate(sourceItem);
        customItem.name = model.UniqueName;
        customItem.settings_Inventory = sourceItem.settings_Inventory.Clone();
        customItem.settings_Inventory.DisplayName = model.DisplayName;
        customItem.settings_Inventory.Description = model.Description;
        AssignGeneratedIcon(customItem, model);
        customItem.settings_buildable = CloneBuildableSettings(sourceItem.settings_buildable, model);
        customItem.settings_recipe = test_balloons_recipes.CreateRecipe(sourceItem, model.UniqueName);

        SetPrivateField(customItem, "uniqueName", model.UniqueName);
        SetPrivateField(customItem, "uniqueIndex", model.UniqueIndex);
        SetPrivateField(customItem, "hasBeenInitialized", true);
        SetPrivateField(customItem, "maxUses", sourceItem.MaxUses);

        AssignItemToBlockPrefabs(customItem);

        return customItem;
    }

    private static void AssignGeneratedIcon(Item_Base customItem, ModelDefinition model)
    {
        if (customItem == null || model == null)
        {
            return;
        }

        Sprite generatedIcon;
        if (!GeneratedIconsByUniqueName.TryGetValue(model.UniqueName, out generatedIcon) || generatedIcon == null)
        {
            generatedIcon = GenerateIconFromModel(model);
            if (generatedIcon != null)
            {
                GeneratedIconsByUniqueName[model.UniqueName] = generatedIcon;
            }
        }

        if (generatedIcon == null)
        {
            return;
        }

        bool assigned = false;
        if (customItem.settings_Inventory != null)
        {
            assigned |= TrySetSpriteMember(customItem.settings_Inventory, generatedIcon);
        }

        assigned |= TrySetSpriteMember(customItem, generatedIcon);

        if (!assigned)
        {
            Debug.LogWarning("Generated icon for " + model.UniqueName + " but could not find a compatible icon field/property to assign.");
        }
    }

    private static bool TrySetSpriteMember(object target, Sprite sprite)
    {
        if (target == null || sprite == null)
        {
            return false;
        }

        string[] memberNames =
        {
            "Icon",
            "icon",
            "Sprite",
            "sprite",
            "ItemIcon",
            "itemIcon",
            "InventorySprite",
            "inventorySprite"
        };

        Type targetType = target.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        for (int index = 0; index < memberNames.Length; index++)
        {
            string memberName = memberNames[index];

            PropertyInfo property = targetType.GetProperty(memberName, flags);
            if (property != null && property.CanWrite && typeof(Sprite).IsAssignableFrom(property.PropertyType))
            {
                property.SetValue(target, sprite);
                return true;
            }

            FieldInfo field = targetType.GetField(memberName, flags);
            if (field != null && typeof(Sprite).IsAssignableFrom(field.FieldType))
            {
                field.SetValue(target, sprite);
                return true;
            }
        }

        return false;
    }

    private static Quaternion GetIconRotation(ModelDefinition model)
    {
        if (model == null)
        {
            return ForwardVisualRotation;
        }

        if (model.UniqueName == TangaroaKitchenBench5SinkItemUniqueName)
        {
            return model.VisualRotation * IconFacingFlipRotation;
        }

        return model.VisualRotation;
    }

    private static Sprite GenerateIconFromModel(ModelDefinition model)
    {
        MeshVisualData visualData;
        if (model == null || !TryGetMeshVisualData(model, out visualData) || visualData == null || visualData.Mesh == null || visualData.Materials == null || visualData.Materials.Length == 0)
        {
            return null;
        }

        GameObject previewRoot = new GameObject("IconPreviewRoot_" + model.UniqueName);
        GameObject cameraObject = new GameObject("IconPreviewCamera_" + model.UniqueName);
        GameObject lightObject = new GameObject("IconPreviewLight_" + model.UniqueName);
        RenderTexture renderTexture = null;
        Camera iconCamera = null;

        try
        {
            previewRoot.hideFlags = HideFlags.HideAndDontSave;
            cameraObject.hideFlags = HideFlags.HideAndDontSave;
            lightObject.hideFlags = HideFlags.HideAndDontSave;

            if (visualData.CompositeTemplate != null && visualData.CompositeTemplate)
            {
                GameObject compositePreview = Object.Instantiate(visualData.CompositeTemplate, previewRoot.transform, false);
                compositePreview.name = "IconCompositeVisual";
                compositePreview.SetActive(true);

                MeshRenderer[] compositeRenderers = compositePreview.GetComponentsInChildren<MeshRenderer>(true);
                for (int index = 0; index < compositeRenderers.Length; index++)
                {
                    compositeRenderers[index].enabled = true;
                }
            }
            else
            {
                MeshFilter meshFilter = previewRoot.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = previewRoot.AddComponent<MeshRenderer>();
                meshFilter.sharedMesh = visualData.Mesh;
                meshRenderer.sharedMaterials = visualData.Materials;
            }

            SetLayerRecursively(previewRoot, IconPreviewLayer);
            previewRoot.transform.rotation = GetIconRotation(model);

            Bounds initialBounds;
            if (!TryGetCombinedRendererBounds(previewRoot, out initialBounds))
            {
                return null;
            }

            previewRoot.transform.position = -initialBounds.center;

            Bounds previewBounds;
            if (!TryGetCombinedRendererBounds(previewRoot, out previewBounds))
            {
                return null;
            }

            Vector3 boundsCenter = previewBounds.center;
            float boundsExtent = Mathf.Max(previewBounds.extents.x, Mathf.Max(previewBounds.extents.y, previewBounds.extents.z));
            boundsExtent = Mathf.Max(0.05f, boundsExtent);

            iconCamera = cameraObject.AddComponent<Camera>();
            iconCamera.clearFlags = CameraClearFlags.SolidColor;
            iconCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            iconCamera.cullingMask = 1 << IconPreviewLayer;
            iconCamera.orthographic = true;
            iconCamera.orthographicSize = boundsExtent * 1.35f;
            iconCamera.nearClipPlane = 0.01f;
            iconCamera.farClipPlane = 20f;

            cameraObject.transform.rotation = Quaternion.Euler(20f, -25f, 0f);
            cameraObject.transform.position = boundsCenter - cameraObject.transform.forward * (boundsExtent * 4f + 1f);

            Light keyLight = lightObject.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 1.15f;
            keyLight.color = Color.white;
            lightObject.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

            renderTexture = new RenderTexture(GeneratedIconSize, GeneratedIconSize, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 2,
                hideFlags = HideFlags.HideAndDontSave
            };

            iconCamera.targetTexture = renderTexture;
            iconCamera.Render();

            RenderTexture previousRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D iconTexture = new Texture2D(GeneratedIconSize, GeneratedIconSize, TextureFormat.RGBA32, false);
            iconTexture.name = model.UniqueName + "_IconTexture";
            iconTexture.ReadPixels(new Rect(0f, 0f, GeneratedIconSize, GeneratedIconSize), 0, 0);
            iconTexture.Apply(false, false);

            RenderTexture.active = previousRenderTexture;

            Sprite iconSprite = Sprite.Create(
                iconTexture,
                new Rect(0f, 0f, iconTexture.width, iconTexture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            iconSprite.name = model.UniqueName + "_Icon";

            GeneratedIconTextures.Add(iconTexture);
            GeneratedIconSprites.Add(iconSprite);

            return iconSprite;
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed generating icon for " + model.UniqueName + "\n" + exception);
            return null;
        }
        finally
        {
            if (renderTexture != null)
            {
                if (iconCamera != null && iconCamera.targetTexture == renderTexture)
                {
                    iconCamera.targetTexture = null;
                }

                if (RenderTexture.active == renderTexture)
                {
                    RenderTexture.active = null;
                }

                Object.DestroyImmediate(renderTexture);
            }

            Object.DestroyImmediate(previewRoot);
            Object.DestroyImmediate(cameraObject);
            Object.DestroyImmediate(lightObject);
        }
    }

    private static void SetLayerRecursively(GameObject target, int layer)
    {
        if (target == null)
        {
            return;
        }

        target.layer = layer;
        Transform transform = target.transform;
        for (int index = 0; index < transform.childCount; index++)
        {
            Transform child = transform.GetChild(index);
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
    }

    private static bool TryGetCombinedRendererBounds(GameObject root, out Bounds bounds)
    {
        bounds = default(Bounds);
        if (root == null)
        {
            return false;
        }

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
        for (int index = 0; index < renderers.Length; index++)
        {
            Renderer renderer = renderers[index];
            if (renderer == null || !renderer.enabled)
            {
                continue;
            }

            Bounds rendererBounds = renderer.bounds;
            // Skip renderers with invalid bounds (NaN or infinite values)
            if (float.IsNaN(rendererBounds.center.x) || float.IsNaN(rendererBounds.center.y) || float.IsNaN(rendererBounds.center.z) ||
                float.IsInfinity(rendererBounds.center.x) || float.IsInfinity(rendererBounds.center.y) || float.IsInfinity(rendererBounds.center.z))
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds = rendererBounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(rendererBounds);
            }
        }

        return hasBounds;
    }

    private static bool TryGetCombinedRendererBoundsLocal(Transform root, out Bounds bounds)
    {
        bounds = default(Bounds);
        if (root == null)
        {
            return false;
        }

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        bool hasBounds = false;
        for (int index = 0; index < renderers.Length; index++)
        {
            Renderer renderer = renderers[index];
            if (renderer == null || !renderer.enabled)
            {
                continue;
            }

            Bounds rendererWorldBounds = renderer.bounds;
            Vector3 center = rendererWorldBounds.center;
            Vector3 extents = rendererWorldBounds.extents;
            
            // Skip renderers with invalid bounds (NaN or infinite values)
            if (float.IsNaN(center.x) || float.IsNaN(center.y) || float.IsNaN(center.z) ||
                float.IsNaN(extents.x) || float.IsNaN(extents.y) || float.IsNaN(extents.z) ||
                float.IsInfinity(center.x) || float.IsInfinity(center.y) || float.IsInfinity(center.z) ||
                float.IsInfinity(extents.x) || float.IsInfinity(extents.y) || float.IsInfinity(extents.z))
            {
                continue;
            }

            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Vector3 worldCorner = center + Vector3.Scale(extents, new Vector3(x, y, z));
                        Vector3 localCorner = root.InverseTransformPoint(worldCorner);

                        if (!hasBounds)
                        {
                            bounds = new Bounds(localCorner, Vector3.zero);
                            hasBounds = true;
                        }
                        else
                        {
                            bounds.Encapsulate(localCorner);
                        }
                    }
                }
            }
        }

        return hasBounds;
    }

    private static void ApplyRecipeMenuLine(Item_Base customItem, int line)
    {
        if (customItem == null || customItem.settings_recipe == null)
        {
            return;
        }

        Traverse.Create(customItem.settings_recipe).Field("subCategory").SetValue(line.ToString());
    }

    private static ItemInstance_Buildable CloneBuildableSettings(ItemInstance_Buildable sourceBuildable, ModelDefinition model)
    {
        if (sourceBuildable == null)
        {
            return null;
        }

        ItemInstance_Buildable clonedBuildable = sourceBuildable.Clone();
        Block[] sourceBlocks = sourceBuildable.GetBlockPrefabs();
        Block[] clonedBlocks = new Block[sourceBlocks.Length];
        Block fallbackBlock = null;

        for (int index = 0; index < sourceBlocks.Length; index++)
        {
            if (sourceBlocks[index] == null)
            {
                continue;
            }

            Block clonedBlock = Object.Instantiate(sourceBlocks[index]);
            if (prefabHolder != null)
            {
                clonedBlock.transform.SetParent(prefabHolder, false);
            }
            clonedBlock.name = model.MeshName + "_Block_" + index;
            clonedBlock.isRotateable = true;
            clonedBlock.canRotateFreely = true;
            clonedBlock.rotateAmount = 90f;
            ApplyCustomModelIfAvailable(clonedBlock.gameObject, model);
            clonedBlocks[index] = clonedBlock;
            if (fallbackBlock == null)
            {
                fallbackBlock = clonedBlock;
            }
        }

        if (fallbackBlock == null)
        {
            return null;
        }

        for (int index = 0; index < clonedBlocks.Length; index++)
        {
            if (clonedBlocks[index] != null)
            {
                continue;
            }

            Block placeholderBlock = Object.Instantiate(fallbackBlock);
            if (prefabHolder != null)
            {
                placeholderBlock.transform.SetParent(prefabHolder, false);
            }
            placeholderBlock.name = model.MeshName + "_Block_" + index + "_Placeholder";
            placeholderBlock.isRotateable = true;
            placeholderBlock.canRotateFreely = true;
            placeholderBlock.rotateAmount = 90f;
            clonedBlocks[index] = placeholderBlock;
        }

        SetPrivateField(clonedBuildable, "blockPrefabs", clonedBlocks);
        return clonedBuildable;
    }

    private static void AssignItemToBlockPrefabs(Item_Base customItem)
    {
        if (customItem == null || customItem.settings_buildable == null)
        {
            return;
        }

        Block[] blockPrefabs = customItem.settings_buildable.GetBlockPrefabs();
        for (int index = 0; index < blockPrefabs.Length; index++)
        {
            if (blockPrefabs[index] == null)
            {
                continue;
            }

            blockPrefabs[index].buildableItem = customItem;
            blockPrefabs[index].itemToReturnOnDestroy = customItem;
        }
    }

    private static bool ApplyCustomModelIfAvailable(GameObject targetObject, ModelDefinition model)
    {
        if (targetObject == null || model == null)
        {
            return false;
        }

        MeshVisualData visualData;
        if (!TryGetMeshVisualData(model, out visualData))
        {
            return false;
        }

        ReplaceVisuals(targetObject, model.UniqueName, visualData, model.VisualRotation);
        ConfigureCollider(targetObject, model.UniqueName, model.QuadTypes);
        ConfigureEaselPaintingSocket(targetObject, model.UniqueName);
        ConfigureWhiteboardDrawing(targetObject, model.UniqueName);
        return true;
    }

    private static void ConfigureEaselPaintingSocket(GameObject targetObject, string uniqueName)
    {
        if (targetObject == null || uniqueName != Easel1ItemUniqueName)
        {
            return;
        }

        Transform socketParent = targetObject.transform;
        Transform customVisual = targetObject.transform.Find("CustomVisual");
        if (customVisual != null)
        {
            socketParent = customVisual;
        }

        Transform existingSocket = socketParent.Find("EaselPaintingSocket");
        if (existingSocket != null)
        {
            Object.Destroy(existingSocket.gameObject);
        }

        SO_BlockQuadType socketQuadType = IsGenericWallEaselQuadEnabled()
            ? GetQuadWallType()
            : GetOrCreateEaselPaintingSocketQuadType();
        if (socketQuadType == null)
        {
            return;
        }

        GameObject socket = new GameObject("EaselPaintingSocket");
        socket.transform.SetParent(socketParent, false);
        EaselSocketProfile profile = GetActiveEaselSocketProfile();
        socket.transform.localPosition = profile.Position;
        socket.transform.localRotation = ComposeEaselSocketLocalRotation(profile.FriendlyEuler);
        socket.transform.localScale = Vector3.one;

        int buildQuadLayer = LayerMask.NameToLayer("BuildQuad");
        if (buildQuadLayer >= 0)
        {
            socket.layer = buildQuadLayer;
        }

        BoxCollider socketCollider = socket.AddComponent<BoxCollider>();
        socketCollider.size = EaselPaintingSocketColliderSize;
        socketCollider.isTrigger = true;

        BlockQuad blockQuad = socket.AddComponent<BlockQuad>();
        blockQuad.quadType = socketQuadType;
        blockQuad.acceptableBuildSides = new[]
        {
            new BlockSurface
            {
                surfaceType = SurfaceType.All,
                dpsType = DPS.Wall
            }
        };
        blockQuad.snapToQuadPosition = true;
        blockQuad.snapToQuadRotation = true;
    }

    private static SO_BlockQuadType GetOrCreateEaselPaintingSocketQuadType()
    {
        if (easelPaintingSocketQuadType == null)
        {
            SO_BlockQuadType quadWallType = GetQuadWallType();
            if (quadWallType == null)
            {
                Debug.LogWarning("Could not load Quad_Wall for easel socket placement.");
                return null;
            }

            easelPaintingSocketQuadType = ScriptableObject.Instantiate(quadWallType);
            easelPaintingSocketQuadType.name = "Quad_EaselPaintingSocket";
        }

        RefreshEaselSocketAcceptedItems(easelPaintingSocketQuadType);
        return easelPaintingSocketQuadType;
    }

    private static SO_BlockQuadType GetQuadWallType()
    {
        SO_BlockQuadType quadWallType = Resources.Load<SO_BlockQuadType>("blockquadtype/Quad_Wall");
        if (quadWallType == null)
        {
            quadWallType = Resources.Load<SO_BlockQuadType>("BlockQuadType/Quad_Wall");
        }

        return quadWallType;
    }

    private static void RefreshEaselSocketAcceptedItems(SO_BlockQuadType quadType)
    {
        if (quadType == null)
        {
            return;
        }

        FieldInfo acceptableField = typeof(SO_BlockQuadType).GetField("acceptableBlockTypes", BindingFlags.Instance | BindingFlags.NonPublic);
        if (acceptableField == null)
        {
            Debug.LogWarning("Could not find acceptableBlockTypes field on SO_BlockQuadType.");
            return;
        }

        HashSet<int> seenUniqueIndexes = new HashSet<int>();
        List<Item_Base> acceptedItems = new List<Item_Base>();
        for (int index = 0; index < EaselSocketAcceptedUniqueNames.Length; index++)
        {
            Item_Base item = ItemManager.GetItemByName(EaselSocketAcceptedUniqueNames[index]);
            if (item == null)
            {
                continue;
            }

            if (seenUniqueIndexes.Add(item.UniqueIndex))
            {
                acceptedItems.Add(item);
            }
        }

        acceptableField.SetValue(quadType, acceptedItems);
    }

    private static void ReplaceVisuals(GameObject targetObject, string uniqueName, MeshVisualData visualData, Quaternion visualRotation)
    {
        if (visualData == null || visualData.Mesh == null || visualData.Materials == null || visualData.Materials.Length == 0)
        {
            return;
        }

        Renderer[] targetRenderers = targetObject.GetComponentsInChildren<Renderer>(true);
        for (int index = 0; index < targetRenderers.Length; index++)
        {
            targetRenderers[index].enabled = false;
        }

        Transform visual = targetObject.transform.Find("CustomVisual");
        if (visual == null)
        {
            GameObject visualObject = new GameObject("CustomVisual");
            visualObject.transform.SetParent(targetObject.transform, false);
            visual = visualObject.transform;
        }

        ClearVisualRoot(visual);

        visual.gameObject.layer = targetObject.layer;
        visual.gameObject.tag = targetObject.tag;

        Vector3 resolvedVisualOffset = GetVisualOffset(uniqueName, visualData.Mesh);
        Quaternion resolvedVisualRotation = visualRotation;
        if (IsPlacedOnEaselPaintingSocket(targetObject, uniqueName))
        {
            resolvedVisualOffset = Vector3.zero;
            resolvedVisualRotation = ForwardVisualRotation;
        }

        visual.localPosition = resolvedVisualOffset;
        visual.localRotation = resolvedVisualRotation;

        visual.localScale = BalloonVisualScale;

        if (TryAttachCompositeVisual(visual, targetObject, visualData))
        {
            MeshFilter anchorFilter = visual.gameObject.AddComponent<MeshFilter>();
            anchorFilter.sharedMesh = visualData.Mesh;
            return;
        }

        MeshFilter mf = visual.gameObject.AddComponent<MeshFilter>();
        mf.sharedMesh = visualData.Mesh;

        MeshRenderer mr = visual.gameObject.AddComponent<MeshRenderer>();
        mr.sharedMaterials = visualData.Materials;
        mr.enabled = true;
    }

    private static void ClearVisualRoot(Transform visualRoot)
    {
        if (visualRoot == null)
        {
            return;
        }

        for (int index = visualRoot.childCount - 1; index >= 0; index--)
        {
            Object.DestroyImmediate(visualRoot.GetChild(index).gameObject);
        }

        Component[] components = visualRoot.GetComponents<Component>();
        for (int index = 0; index < components.Length; index++)
        {
            Component component = components[index];
            if (component != null && !(component is Transform))
            {
                Object.DestroyImmediate(component);
            }
        }
    }

    private static bool TryAttachCompositeVisual(Transform visualRoot, GameObject targetObject, MeshVisualData visualData)
    {
        if (visualRoot == null || targetObject == null || visualData == null || visualData.CompositeTemplate == null)
        {
            return false;
        }

        if (!visualData.CompositeTemplate)
        {
            return false;
        }

        GameObject compositeVisual = Object.Instantiate(visualData.CompositeTemplate, visualRoot, false);
        compositeVisual.name = "CompositeVisual";
        compositeVisual.SetActive(true);

        MeshRenderer[] renderers = compositeVisual.GetComponentsInChildren<MeshRenderer>(true);
        for (int index = 0; index < renderers.Length; index++)
        {
            renderers[index].enabled = true;
        }

        Transform[] transforms = compositeVisual.GetComponentsInChildren<Transform>(true);
        for (int index = 0; index < transforms.Length; index++)
        {
            transforms[index].gameObject.layer = targetObject.layer;
            transforms[index].gameObject.tag = targetObject.tag;
        }

        return compositeVisual.GetComponentInChildren<MeshRenderer>(true) != null;
    }

    private static bool IsPlacedOnEaselPaintingSocket(GameObject targetObject, string uniqueName)
    {
        if (targetObject == null)
        {
            return false;
        }

        Transform transform = targetObject.transform;
        if (transform == null)
        {
            return false;
        }

        Transform parent = transform.parent;
        if (parent == null)
        {
            return false;
        }

        if (parent.name == "EaselPaintingSocket")
        {
            return true;
        }

        BlockQuad parentQuad = parent.GetComponent<BlockQuad>();
        if (parentQuad != null && parentQuad.quadType != null && parentQuad.quadType.name == "Quad_EaselPaintingSocket")
        {
            return true;
        }

        Transform[] ancestry = transform.GetComponentsInParent<Transform>(true);
        for (int index = 0; index < ancestry.Length; index++)
        {
            if (ancestry[index] != null && ancestry[index].name == "EaselPaintingSocket")
            {
                return true;
            }
        }

        return false;
    }

    private static void ApplyEaselSocketTransformToAllInstances(string uniqueName = null)
    {
        EaselSocketProfile profile = GetActiveEaselSocketProfile(uniqueName);
        Vector3 position = profile.Position;
        Quaternion rotation = ComposeEaselSocketLocalRotation(profile.FriendlyEuler);
        SO_BlockQuadType socketQuadType = IsGenericWallEaselQuadEnabled()
            ? GetQuadWallType()
            : GetOrCreateEaselPaintingSocketQuadType();

        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int index = 0; index < transforms.Length; index++)
        {
            Transform transform = transforms[index];
            if (transform == null || transform.name != "EaselPaintingSocket")
            {
                continue;
            }

            transform.localPosition = position;
            transform.localRotation = rotation;

            BlockQuad blockQuad = transform.GetComponent<BlockQuad>();
            if (blockQuad != null && socketQuadType != null)
            {
                blockQuad.quadType = socketQuadType;
            }
        }

        EaselPaintingSocketLocalPosition = position;
        EaselPaintingSocketFriendlyEuler = profile.FriendlyEuler;
        EaselPaintingSocketLocalRotation = rotation;
    }

    private static bool IsGenericWallEaselQuadEnabled()
    {
        return instance != null && instance.ExtraSettings_UseGenericWallEaselQuad;
    }

    private static Block FindMountedBlockOnEasel(Block easelBlock)
    {
        if (easelBlock == null)
        {
            return null;
        }

        Transform socketTransform = easelBlock.transform.Find("CustomVisual/EaselPaintingSocket");
        if (socketTransform == null)
        {
            socketTransform = easelBlock.transform.Find("EaselPaintingSocket");
        }

        if (socketTransform == null)
        {
            return null;
        }

        Block[] mountedBlocks = socketTransform.GetComponentsInChildren<Block>(true);
        for (int index = 0; index < mountedBlocks.Length; index++)
        {
            Block mountedBlock = mountedBlocks[index];
            if (mountedBlock == null || mountedBlock == easelBlock)
            {
                continue;
            }

            return mountedBlock;
        }

        return null;
    }

    private static bool TryInvokeBlockRemoval(object blockCreatorInstance, Block mountedBlock)
    {
        if (blockCreatorInstance == null || mountedBlock == null)
        {
            return false;
        }

        MethodInfo[] methods = blockCreatorInstance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(method => method.Name == "RemoveBlock")
            .ToArray();

        for (int methodIndex = 0; methodIndex < methods.Length; methodIndex++)
        {
            MethodInfo method = methods[methodIndex];
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length == 0 || !typeof(Block).IsAssignableFrom(parameters[0].ParameterType))
            {
                continue;
            }

            object[] args = new object[parameters.Length];
            args[0] = mountedBlock;
            for (int parameterIndex = 1; parameterIndex < parameters.Length; parameterIndex++)
            {
                Type parameterType = parameters[parameterIndex].ParameterType;
                if (parameterType == typeof(bool))
                {
                    args[parameterIndex] = true;
                }
                else if (parameterType.IsValueType)
                {
                    args[parameterIndex] = Activator.CreateInstance(parameterType);
                }
                else
                {
                    args[parameterIndex] = null;
                }
            }

            try
            {
                method.Invoke(blockCreatorInstance, args);
                return true;
            }
            catch
            {
                // Try
            }
        }

        return false;
    }

    private static EaselSocketProfile GetActiveEaselSocketProfile(string uniqueName = null)
    {
        if (!IsGenericWallEaselQuadEnabled())
        {
            return GetEaselSocketProfile(uniqueName);
        }

        // Keep painting/canvas per-item easel tuning even when using a generic wall quad.
        if (IsEaselSocketItem(uniqueName))
        {
            return GetEaselSocketProfile(uniqueName);
        }

        return GenericWallEaselSocketProfile;
    }

    private static Quaternion ComposeEaselSocketLocalRotation(Vector3 friendlyEuler)
    {
        return EaselPaintingSocketRotationCompensation * Quaternion.Euler(friendlyEuler);
    }

    private static Vector3 ExtractEaselSocketFriendlyEuler(Quaternion rawSocketRotation)
    {
        Quaternion friendly = Quaternion.Inverse(EaselPaintingSocketRotationCompensation) * rawSocketRotation;
        return friendly.eulerAngles;
    }

    private static void SetEaselSocketFriendlyRotation(Vector3 friendlyEuler)
    {
        EaselPaintingSocketFriendlyEuler = friendlyEuler;
        EaselPaintingSocketLocalRotation = ComposeEaselSocketLocalRotation(friendlyEuler);
    }

    private static EaselSocketProfile GetEaselSocketProfile(string uniqueName = null)
    {
        if (!string.IsNullOrEmpty(uniqueName) && EaselSocketProfilesByItem.TryGetValue(uniqueName, out EaselSocketProfile storedProfile))
        {
            return storedProfile;
        }

        return EaselSocketProfileState;
    }

    private static void SetEaselSocketProfile(string uniqueName, EaselSocketProfile profile)
    {
        if (!string.IsNullOrEmpty(uniqueName) && EaselSocketAcceptedUniqueNames.Contains(uniqueName))
        {
            EaselSocketProfilesByItem[uniqueName] = profile;
        }
        else
        {
            EaselSocketProfileState = profile;
            for (int index = 0; index < EaselSocketAcceptedUniqueNames.Length; index++)
            {
                EaselSocketProfilesByItem[EaselSocketAcceptedUniqueNames[index]] = profile;
            }
        }

        EaselPaintingSocketLocalPosition = profile.Position;
        EaselPaintingSocketFriendlyEuler = profile.FriendlyEuler;
        EaselPaintingSocketLocalRotation = ComposeEaselSocketLocalRotation(profile.FriendlyEuler);
    }

    private static bool TryResolveEaselSocketItemKey(string key, out string uniqueName)
    {
        uniqueName = null;
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        string normalized = key.Trim().ToLowerInvariant();
        if (normalized == "painting1" || normalized == "p1")
        {
            uniqueName = Painting1ItemUniqueName;
            return true;
        }

        if (normalized == "painting2" || normalized == "p2")
        {
            uniqueName = Painting2ItemUniqueName;
            return true;
        }

        if (normalized == "painting3" || normalized == "p3")
        {
            uniqueName = Painting3ItemUniqueName;
            return true;
        }

        if (normalized == "canvas" || normalized == "blankcanvas" || normalized == "canvasblank")
        {
            uniqueName = CanvasBlankItemUniqueName;
            return true;
        }

        return false;
    }

    private static string GetEaselSocketDebugLabel(string uniqueName)
    {
        if (uniqueName == Painting1ItemUniqueName)
        {
            return "painting1";
        }

        if (uniqueName == Painting2ItemUniqueName)
        {
            return "painting2";
        }

        if (uniqueName == Painting3ItemUniqueName)
        {
            return "painting3";
        }

        if (uniqueName == CanvasBlankItemUniqueName)
        {
            return "canvas";
        }

        return uniqueName ?? "unknown";
    }

    private static bool TryGetMeshVisualData(ModelDefinition model, out MeshVisualData visualData)
    {
        if (model != null && model.HasExplicitSource)
        {
            return TryGetMeshVisualDataByIdentity(new MeshIdentity(model.SourceAssetFile, model.SourcePathId), model.MeshName, out visualData);
        }

        return TryGetMeshVisualDataByName(model != null ? model.MeshName : null, out visualData);
    }

    private static bool TryGetMeshVisualDataByIdentity(MeshIdentity identity, string meshNameForLogging, out MeshVisualData visualData)
    {
        if (PreloadedVisualsByIdentity.TryGetValue(identity, out visualData) && visualData != null
            && visualData.Mesh != null && visualData.Materials != null && visualData.Materials.Length > 0)
        {
            return true;
        }

        //Debug.LogWarning("No precise mesh cached for \"" + meshNameForLogging + "\" (" + identity +
        //    "). It needs to go through PreloadExplicitSourceMeshesAsync before first use.");
        visualData = null;
        return false;
    }

    private static bool TryGetMeshVisualDataByName(string meshName, out MeshVisualData visualData)
    {
        if (string.IsNullOrEmpty(meshName))
        {
            visualData = null;
            return false;
        }

        MeshVisualData existingData;
        if (PreloadedVisuals.TryGetValue(meshName, out existingData) && existingData != null)
        {
            bool hasBaseVisual = existingData.Mesh != null && existingData.Materials != null && existingData.Materials.Length > 0;
            if (!CompositeMeshNames.Contains(meshName))
            {
                visualData = existingData;
                return hasBaseVisual;
            }

            if (hasBaseVisual && CountTemplateRenderers(existingData.CompositeTemplate) >= 2)
            {
                visualData = existingData;
                return true;
            }
        }

        MeshVisualData bestCandidate = existingData;
        int bestRendererCount = CountTemplateRenderers(existingData != null ? existingData.CompositeTemplate : null);

        MeshFilter[] meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();
        for (int index = 0; index < meshFilters.Length; index++)
        {
            MeshFilter meshFilter = meshFilters[index];
            if (meshFilter == null || meshFilter.sharedMesh == null || meshFilter.sharedMesh.name != meshName)
            {
                continue;
            }

            Renderer renderer = meshFilter.GetComponent<Renderer>();
            if (renderer == null || renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
            {
                continue;
            }

            GameObject compositeTemplate = BuildCompositeTemplateIfNeeded(meshName, meshFilter.transform);
            int candidateRendererCount = CountTemplateRenderers(compositeTemplate);

            if (!CompositeMeshNames.Contains(meshName))
            {
                visualData = new MeshVisualData(meshFilter.sharedMesh, renderer.sharedMaterials, compositeTemplate);
                PreloadedVisuals[meshName] = visualData;
                return true;
            }

            if (candidateRendererCount > bestRendererCount)
            {
                bestRendererCount = candidateRendererCount;
                bestCandidate = new MeshVisualData(meshFilter.sharedMesh, renderer.sharedMaterials, compositeTemplate);
            }

            if (bestRendererCount >= 2)
            {
                break;
            }
        }

        if (bestCandidate != null && bestCandidate.Mesh != null && bestCandidate.Materials != null && bestCandidate.Materials.Length > 0)
        {
            visualData = bestCandidate;
            PreloadedVisuals[meshName] = bestCandidate;
            return true;
        }

        visualData = null;
        return false;
    }

    private static bool TryFindMeshVisual(string meshName, out Mesh mesh, out Material[] materials)
    {
        MeshVisualData preloadedData;
        if (TryGetMeshVisualDataByName(meshName, out preloadedData))
        {
            mesh = preloadedData.Mesh;
            materials = preloadedData.Materials;
            return true;
        }

        mesh = null;
        materials = null;
        return false;
    }

    private static void ConfigureCollider(GameObject targetObject, string uniqueName, RBlockQuadType[] supportedQuadTypes)
    {
        Collider[] existingColliders = targetObject.GetComponentsInChildren<Collider>(true);
        for (int index = 0; index < existingColliders.Length; index++)
        {
            if (IsPlacementOverlapCollider(existingColliders[index]))
            {
                continue;
            }

            existingColliders[index].enabled = false;
            Object.Destroy(existingColliders[index]);
        }

        Transform visual = targetObject.transform.Find("CustomVisual");
        if (visual == null)
        {
            return;
        }

        int blockLayer = LayerMask.NameToLayer("Block");
        if (blockLayer >= 0)
        {
            targetObject.layer = blockLayer;
            SetLayerRecursively(visual.gameObject, blockLayer);
        }

        if (targetObject.CompareTag("IgnoreRemovePlaceables"))
        {
            targetObject.tag = "Untagged";
        }

        if (visual.gameObject.CompareTag("IgnoreRemovePlaceables"))
        {
            visual.gameObject.tag = "Untagged";
        }

        // Special handling for items with custom collider sets
        if (ColliderManager.HasColliderSet(uniqueName))
        {
            MeshFilter specialMeshFilter = visual.GetComponent<MeshFilter>();
            Mesh specialMesh = specialMeshFilter != null ? specialMeshFilter.sharedMesh : null;
            ColliderManager.ApplySpecialColliders(targetObject, uniqueName, specialMesh);
            AttachCollisionGate(targetObject, uniqueName);
            return;
        }

        MeshFilter visualMeshFilter = visual.GetComponent<MeshFilter>();
        Mesh mesh = visualMeshFilter != null ? visualMeshFilter.sharedMesh : null;
        bool canUseMeshCollider = false;
        if (mesh != null)
        {
            try
            {
                canUseMeshCollider = mesh.isReadable;
            }
            catch
            {
                canUseMeshCollider = false;
            }
        }

        Bounds meshBounds;
        if (mesh != null)
        {
            meshBounds = mesh.bounds;
        }
        else if (!TryGetCombinedRendererBoundsLocal(visual, out meshBounds))
        {
            return;
        }

        bool isTablePlacement = supportedQuadTypes != null && System.Array.Exists(supportedQuadTypes, element => element == RBlockQuadType.quad_table);
        bool isEaselSocketPlacement = IsPlacedOnEaselPaintingSocket(targetObject, uniqueName);

        if (!ColliderManager.HasColliderSet(uniqueName))
        {
            ApplyMeshBoundsToAdvancedCollision(targetObject, visual, meshBounds, uniqueName, isTablePlacement, isEaselSocketPlacement);
        }

        Vector3 centerOffset = GetColliderCenterOffset(uniqueName, isTablePlacement, isEaselSocketPlacement);

        BoxCollider boxCollider = targetObject.AddComponent<BoxCollider>();
        boxCollider.center = visual.localPosition + meshBounds.center + centerOffset;
        boxCollider.size = GetScaledPlacementSize(meshBounds.size, uniqueName);
        boxCollider.isTrigger = false;
        if (blockLayer >= 0)
        {
            boxCollider.gameObject.layer = blockLayer;
        }

        if (canUseMeshCollider && mesh != null)
        {
            MeshCollider meshCollider = visual.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = false;
            meshCollider.isTrigger = false;
            if (blockLayer >= 0)
            {
                meshCollider.gameObject.layer = blockLayer;
            }
        }

        AttachCollisionGate(targetObject, uniqueName);
    }

    private static void AttachCollisionGate(GameObject targetObject, string uniqueName)
    {
        if (targetObject == null || !CustomUniqueNames.Contains(uniqueName))
        {
            return;
        }

        CollisionGateUntilPlaced gate = targetObject.GetComponent<CollisionGateUntilPlaced>();
        if (gate == null)
        {
            gate = targetObject.AddComponent<CollisionGateUntilPlaced>();
        }

        gate.Initialize();
    }



    private static void ApplyMeshBoundsToAdvancedCollision(GameObject rootObject, Transform visual, Bounds meshBounds, string uniqueName, bool isTablePlacement, bool isEaselSocketPlacement)
    {
        AdvancedCollision[] advancedCollisions = rootObject.GetComponentsInChildren<AdvancedCollision>(true);
        for (int index = 0; index < advancedCollisions.Length; index++)
        {
            AdvancedCollision advancedCollision = advancedCollisions[index];
            if (advancedCollision == null)
            {
                continue;
            }

            BoxCollider[] colliders = advancedCollision.GetComponents<BoxCollider>();
            if (colliders == null || colliders.Length == 0)
            {
                colliders = new[] { advancedCollision.gameObject.AddComponent<BoxCollider>() };
            }

            for (int colliderIndex = 0; colliderIndex < colliders.Length; colliderIndex++)
            {
                BoxCollider collider = colliders[colliderIndex];
                if (collider == null)
                {
                    continue;
                }

                Vector3 localCenter;
                Vector3 localSize;
                ConvertBoundsToLocalSpace(visual, meshBounds, collider.transform, out localCenter, out localSize);
                Vector3 centerOffset = GetColliderCenterOffset(uniqueName, isTablePlacement, isEaselSocketPlacement);
                Vector3 worldOffset = rootObject.transform.TransformVector(centerOffset);
                collider.center = localCenter + collider.transform.InverseTransformVector(worldOffset);
                collider.size = GetScaledPlacementSize(localSize, uniqueName);
                collider.isTrigger = false;
            }
        }
    }

    private static Vector3 GetColliderCenterOffset(string uniqueName, bool isTablePlacement, bool isEaselSocketPlacement)
    {
        if (isTablePlacement)
        {
            return new Vector3(0f, 0.01f, 0f);
        }

        if (uniqueName == CanvasBlankItemUniqueName && !isEaselSocketPlacement)
        {
            return new Vector3(0f, 0f, CanvasBlankVisualOffset.z);
        }

        if (uniqueName == RugItemUniqueName || uniqueName == Rug03ItemUniqueName)
        {
            return new Vector3(0f, 0.02f, 0f);
        }

        if (uniqueName == HedgeBush1ItemUniqueName || uniqueName == HedgeBush2ItemUniqueName)
        {
            return new Vector3(0f, 0.10f, 0f);
        }

        if (uniqueName == TangaroaPaperPile1ItemUniqueName)
        {
            return new Vector3(0f, 0.04f, 0f);
        }

        if (uniqueName == SignDoorSurfaceAccessItemUniqueName)
        {
            return SurfaceAccessVisualOffset;
        }

        return Vector3.zero;
    }

    private static Vector3 GetScaledPlacementSize(Vector3 sourceSize, string uniqueName)
    {
        ModelDefinition model;
        float scale = ModelsByUniqueName.TryGetValue(uniqueName, out model) ? model.PlacementScale : PlacementColliderScale;
        Vector3 axisScale = model != null ? model.ColliderAxisScale : DefaultColliderAxisScale;
        Vector3 scaled = Vector3.Scale(sourceSize * scale, axisScale);
        scaled.x = Mathf.Max(MinPlacementColliderAxis, scaled.x);
        scaled.y = Mathf.Max(MinPlacementColliderAxis, scaled.y);
        scaled.z = Mathf.Max(MinPlacementColliderAxis, scaled.z);
        return scaled;
    }

    private static void ConvertBoundsToLocalSpace(Transform sourceTransform, Bounds sourceBounds, Transform targetTransform, out Vector3 localCenter, out Vector3 localSize)
    {
        Vector3 extents = sourceBounds.extents;
        Vector3 sourceCenter = sourceBounds.center;

        Vector3[] localCorners = new Vector3[8];
        int cornerIndex = 0;
        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 corner = sourceCenter + Vector3.Scale(extents, new Vector3(x, y, z));
                    Vector3 worldCorner = sourceTransform.TransformPoint(corner);
                    localCorners[cornerIndex++] = targetTransform.InverseTransformPoint(worldCorner);
                }
            }
        }

        Vector3 min = localCorners[0];
        Vector3 max = localCorners[0];
        for (int index = 1; index < localCorners.Length; index++)
        {
            min = Vector3.Min(min, localCorners[index]);
            max = Vector3.Max(max, localCorners[index]);
        }

        localCenter = (min + max) * 0.5f;
        localSize = max - min;
    }

    private static bool IsPlacementOverlapCollider(Collider collider)
    {
        if (collider == null)
        {
            return false;
        }

        if (collider.GetComponent<SpecialItemCollider>() != null)
        {
            return false;
        }

        return collider is BoxCollider && collider.GetComponent<AdvancedCollision>() != null;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }

    public void OnModUnload()
    {
        Debug.Log("Mod test-balloons has been unloaded!");
        instance = null;

        harmony?.UnpatchAll("com.test_balloons.mod");
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (preloadRoutine != null)
        {
            StopCoroutine(preloadRoutine);
            preloadRoutine = null;
        }

        if (prefabHolder != null)
        {
            Object.Destroy(prefabHolder.gameObject);
            prefabHolder = null;
        }

        PreloadedVisuals.Clear();
        PreloadedVisualsByIdentity.Clear();
        if (easelPaintingSocketQuadType != null)
        {
            Object.Destroy(easelPaintingSocketQuadType);
            easelPaintingSocketQuadType = null;
        }

        for (int spriteIndex = 0; spriteIndex < GeneratedIconSprites.Count; spriteIndex++)
        {
            if (GeneratedIconSprites[spriteIndex] != null)
            {
                Object.Destroy(GeneratedIconSprites[spriteIndex]);
            }
        }

        for (int textureIndex = 0; textureIndex < GeneratedIconTextures.Count; textureIndex++)
        {
            if (GeneratedIconTextures[textureIndex] != null)
            {
                Object.Destroy(GeneratedIconTextures[textureIndex]);
            }
        }

        GeneratedIconSprites.Clear();
        GeneratedIconTextures.Clear();
        GeneratedIconsByUniqueName.Clear();
        preloadStarted = false;
        preloadCompleted = false;

        for (int index = 0; index < registeredItems.Count; index++)
        {
            if (registeredItems[index] != null)
            {
                RAPI.UnregisterItem(registeredItems[index]);
            }
        }

        registeredItems.Clear();
    }

#if false // Debug Commands - Disabled
    [ConsoleCommand("decor.adjustsocket")]
    static void DebugAdjustSocket(string[] args)
    {
        if (args == null || args.Length < 1)
        {
            Debug.Log("Usage: decor.adjustsocket <command> [x] [y] [z]");
            Debug.Log("Commands: getpos, getrot, setpos x y z, addpos x y z, setrot x y z");
            return;
        }

        bool LogTryParse(string val, out float value)
        {
            if (float.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value))
                return true;
            Debug.LogError($"Failed to parse '{val}' as float");
            return false;
        }

        if (args[0] == "getpos")
        {
            EaselSocketProfile profile = GetEaselSocketProfile();
            Debug.Log("Easel Socket Target: generic");
            Debug.Log($"Easel Painting Socket Position: {profile.Position}");
        }
        else if (args[0] == "getrot")
        {
            EaselSocketProfile profile = GetEaselSocketProfile();
            Debug.Log("Easel Socket Target: generic");
            Debug.Log($"Easel Painting Socket Friendly Rotation (Euler): {profile.FriendlyEuler}");
            Debug.Log($"Easel Painting Socket Internal Rotation (Euler): {ComposeEaselSocketLocalRotation(profile.FriendlyEuler).eulerAngles}");
        }
        else if (args[0] == "getrotraw")
        {
            EaselSocketProfile profile = GetEaselSocketProfile();
            Quaternion rawRotation = ComposeEaselSocketLocalRotation(profile.FriendlyEuler);
            Debug.Log("Easel Socket Target: generic");
            Debug.Log($"Easel Painting Socket Raw Rotation (Euler): {rawRotation.eulerAngles}");
            Debug.Log($"Easel Painting Socket Raw Rotation (Quat): {rawRotation}");
        }
        else if (args[0] == "target")
        {
            Debug.Log("Targeting is optional now. Socket tuning commands apply to a shared generic socket profile.");
        }
        else if (args[0] == "gettarget")
        {
            Debug.Log("Easel socket target: generic");
        }
        else if (args[0] == "setpos" || args[0] == "addpos" || args[0] == "setrot" || args[0] == "addrot" || args[0] == "setrotraw")
        {
            if (args.Length < 4 || !LogTryParse(args[1], out var x) || !LogTryParse(args[2], out var y) || !LogTryParse(args[3], out var z))
            {
                Debug.LogError("Invalid arguments. Usage: decor.adjustsocket <setpos|addpos|setrot|addrot|setrotraw> x y z");
                return;
            }

            EaselSocketProfile profile = GetEaselSocketProfile();

            if (args[0] == "setrot" || args[0] == "addrot" || args[0] == "setrotraw")
            {
                if (args[0] == "setrotraw")
                {
                    profile.FriendlyEuler = ExtractEaselSocketFriendlyEuler(Quaternion.Euler(x, y, z));
                }
                else if (args[0] == "setrot")
                {
                    profile.FriendlyEuler = new Vector3(x, y, z);
                }
                else
                {
                    profile.FriendlyEuler += new Vector3(x, y, z);
                }

                SetEaselSocketProfile(null, profile);
                ApplyEaselSocketTransformToAllInstances();
                Debug.Log("Socket target: generic");
                Debug.Log($"Socket friendly rotation set to: {profile.FriendlyEuler}");
                Debug.Log($"Socket internal rotation now: {ComposeEaselSocketLocalRotation(profile.FriendlyEuler).eulerAngles}");
            }
            else
            {
                profile.Position = args[0] == "setpos"
                    ? new Vector3(x, y, z)
                    : profile.Position + new Vector3(x, y, z);

                SetEaselSocketProfile(null, profile);
                ApplyEaselSocketTransformToAllInstances();
                Debug.Log("Socket target: generic");
                Debug.Log($"Socket position set to: {profile.Position}");
            }
        }
        else if (args[0] == "help")
        {
            Debug.Log("Usage: decor.adjustsocket <command> [x] [y] [z]");
            Debug.Log("Commands: gettarget, target <painting1|painting2|painting3|canvas>, getpos, getrot, getrotraw, setpos x y z, addpos x y z, setrot x y z, addrot x y z, setrotraw x y z");
        }
        else
        {
            Debug.LogError($"Unknown command: {args[0]}");
        }
    }
#endif

    [HarmonyPatch(typeof(BlockCreator), "RotateBlock")]
    static class Patch_BlockCreator_RotateBlock
    {
        static void Prefix(Block block, ref float degrees, bool snap)
        {
            if (!snap && block != null && block.buildableItem != null && CustomUniqueNames.Contains(block.buildableItem.UniqueName))
            {
                degrees = -degrees;
            }
        }
    }

    [HarmonyPatch(typeof(BlockCreator), "RemoveBlock")]
    static class Patch_BlockCreator_RemoveBlock_EaselMountedCleanup
    {
        static void Prefix(object __instance, Block block)
        {
            if (!IsGenericWallEaselQuadEnabled() || block == null || block.buildableItem == null || block.buildableItem.UniqueName != Easel1ItemUniqueName)
            {
                return;
            }

            Block mountedBlock = FindMountedBlockOnEasel(block);
            if (mountedBlock == null)
            {
                return;
            }

            if (!TryInvokeBlockRemoval(__instance, mountedBlock))
            {
                Debug.LogWarning("[SID] Failed to remove mounted easel item through BlockCreator.RemoveBlock; destroying mounted block to avoid floating orphan.");
                Object.Destroy(mountedBlock.gameObject);
            }
        }
    }

    [HarmonyPatch(typeof(BlockCreator), "GetQuadAtCursor")]
    static class Patch_BlockCreator_GetQuadAtCursor_EaselColliderRedirect
    {
        private static readonly FieldInfo SelectedBuildableItemField = typeof(BlockCreator).GetField("selectedBuildableItem", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo QuadHitField = typeof(BlockCreator).GetField("quadHit", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo QuadSurfaceField = typeof(BlockCreator).GetField("quadSurface", BindingFlags.Instance | BindingFlags.NonPublic);

        static void Postfix(BlockCreator __instance, ref BlockQuad __result)
        {
            if (__instance == null)
            {
                return;
            }

            Item_Base selectedBuildableItem = SelectedBuildableItemField != null
                ? SelectedBuildableItemField.GetValue(__instance) as Item_Base
                : null;
            if (selectedBuildableItem == null)
            {
                return;
            }

            if (!IsGenericWallEaselQuadEnabled() && !IsEaselSocketItem(selectedBuildableItem.UniqueName))
            {
                return;
            }

            RaycastHit cursorHit;
            if (!Helper.HitAtCursor(out cursorHit, Player.UseDistance * 2f, LayerMasks.MASK_IgnorePlayer, QueryTriggerInteraction.Collide) || cursorHit.collider == null)
            {
                return;
            }

            Block aimedBlock = cursorHit.collider.GetComponentInParent<Block>();
            if (aimedBlock == null || aimedBlock.buildableItem == null || aimedBlock.buildableItem.UniqueName != Easel1ItemUniqueName)
            {
                return;
            }

            BlockQuad socketQuad = FindEaselSocketQuad(aimedBlock.transform);
            if (socketQuad == null)
            {
                return;
            }

            EaselSocketProfile selectedProfile = GetActiveEaselSocketProfile(IsEaselSocketItem(selectedBuildableItem.UniqueName) ? selectedBuildableItem.UniqueName : null);
            socketQuad.transform.localPosition = selectedProfile.Position;
            socketQuad.transform.localRotation = ComposeEaselSocketLocalRotation(selectedProfile.FriendlyEuler);

            Vector3 socketNormal = socketQuad.transform.forward;
            RaycastHit redirectedHit = cursorHit;
            redirectedHit.point = socketQuad.transform.position;
            redirectedHit.normal = socketNormal;

            if (QuadHitField != null)
            {
                QuadHitField.SetValue(__instance, redirectedHit);
            }

            if (QuadSurfaceField != null)
            {
                QuadSurfaceField.SetValue(__instance, socketQuad.GetSurfaceFromNormal(socketNormal));
            }

            __result = socketQuad;
        }
    }

    private static BlockQuad FindEaselSocketQuad(Transform easelRoot)
    {
        if (easelRoot == null)
        {
            return null;
        }

        Transform socketTransform = easelRoot.Find("CustomVisual/EaselPaintingSocket");
        if (socketTransform == null)
        {
            socketTransform = easelRoot.Find("EaselPaintingSocket");
        }

        if (socketTransform != null)
        {
            BlockQuad directQuad = socketTransform.GetComponent<BlockQuad>();
            if (directQuad != null)
            {
                return directQuad;
            }
        }

        BlockQuad[] childQuads = easelRoot.GetComponentsInChildren<BlockQuad>(true);
        for (int index = 0; index < childQuads.Length; index++)
        {
            BlockQuad quad = childQuads[index];
            if (quad == null)
            {
                continue;
            }

            if (quad.name == "EaselPaintingSocket")
            {
                return quad;
            }

            if (quad.quadType != null && quad.quadType.name == "Quad_EaselPaintingSocket")
            {
                return quad;
            }
        }

        return null;
    }

    [HarmonyPatch(typeof(Helper), "GetTerm")]
    static class Patch_Helper_GetTerm_CustomCraftingLabel
    {
        static bool Prefix(string term, bool applyParameters, ref string __result)
        {
            if (term == "Crafting/" + CustomCraftingCategoryId)
            {
                __result = CustomCraftingCategoryLabel;
                return false;
            }

            return true;
        }
    }

    private class CollisionGateUntilPlaced : MonoBehaviour
    {
        private readonly List<Collider> nonOverlapColliders = new List<Collider>();
        private Block block;

        public void Initialize()
        {
            block = GetComponent<Block>();
            CacheNonOverlapColliders();

            if (IsPlacedInWorld(block))
            {
                SetCollidersEnabled(true);
                enabled = false;
                return;
            }

            SetCollidersEnabled(false);
            enabled = true;
        }

        private void Update()
        {
            if (!IsPlacedInWorld(block))
            {
                return;
            }

            SetCollidersEnabled(true);
            enabled = false;
        }

        private void CacheNonOverlapColliders()
        {
            nonOverlapColliders.Clear();
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            for (int index = 0; index < colliders.Length; index++)
            {
                Collider collider = colliders[index];
                if (collider == null || IsPlacementOverlapCollider(collider))
                {
                    continue;
                }

                nonOverlapColliders.Add(collider);
            }
        }

        private void SetCollidersEnabled(bool state)
        {
            for (int index = 0; index < nonOverlapColliders.Count; index++)
            {
                Collider collider = nonOverlapColliders[index];
                if (collider != null)
                {
                    collider.enabled = state;
                }
            }
        }

        private static bool IsPlacedInWorld(Block block)
        {
            if (block == null)
            {
                return false;
            }

            List<Block> placedBlocks = BlockCreator.GetPlacedBlocks();
            return placedBlocks != null && placedBlocks.Contains(block);
        }
    }

    private readonly struct MeshIdentity : IEquatable<MeshIdentity>
    {
        public readonly string AssetFile;
        public readonly long PathId;

        public MeshIdentity(string assetFile, long pathId)
        {
            AssetFile = assetFile;
            PathId = pathId;
        }

        public bool Equals(MeshIdentity other)
        {
            return PathId == other.PathId && string.Equals(AssetFile, other.AssetFile, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is MeshIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((AssetFile != null ? AssetFile.GetHashCode() : 0) * 397) ^ PathId.GetHashCode();
            }
        }

        public override string ToString()
        {
            return AssetFile + "#" + PathId;
        }
    }

    private class ModelDefinition
    {
        public readonly string UniqueName;
        public readonly int UniqueIndex;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly string MeshName;
        public readonly string SourceAssetFile;
        public readonly long SourcePathId;
        public readonly bool HasExplicitSource;
        public readonly Quaternion VisualRotation;
        public readonly float PlacementScale;
        public readonly bool CenterVerticallyOnMesh;
        public readonly Vector3 ColliderAxisScale;
        public readonly RBlockQuadType[] QuadTypes;

        public ModelDefinition(
            string uniqueName,
            int uniqueIndex,
            string displayName,
            string description,
            string meshName,
            Quaternion visualRotation,
            float placementScale,
            bool centerVerticallyOnMesh,
            Vector3 colliderAxisScale,
            params RBlockQuadType[] quadTypes)
        {
            UniqueName = uniqueName;
            UniqueIndex = uniqueIndex;
            DisplayName = displayName;
            Description = description;
            MeshName = meshName;
            VisualRotation = visualRotation;
            PlacementScale = placementScale;
            CenterVerticallyOnMesh = centerVerticallyOnMesh;
            ColliderAxisScale = colliderAxisScale;
            QuadTypes = quadTypes;
            SourceAssetFile = null;
            SourcePathId = 0;
            HasExplicitSource = false;
        }

        public ModelDefinition(
            string uniqueName,
            int uniqueIndex,
            string displayName,
            string description,
            string meshName,
            Quaternion visualRotation,
            float placementScale,
            bool centerVerticallyOnMesh,
            params RBlockQuadType[] quadTypes)
            : this(uniqueName, uniqueIndex, displayName, description, meshName, visualRotation, placementScale, centerVerticallyOnMesh, DefaultColliderAxisScale, quadTypes)
        {
        }

        public ModelDefinition(
            string uniqueName,
            int uniqueIndex,
            string displayName,
            string description,
            string meshName,
            string sourceAssetFile,
            long sourcePathId,
            Quaternion visualRotation,
            float placementScale,
            bool centerVerticallyOnMesh,
            Vector3 colliderAxisScale,
            params RBlockQuadType[] quadTypes)
            : this(uniqueName, uniqueIndex, displayName, description, meshName, visualRotation, placementScale, centerVerticallyOnMesh, colliderAxisScale, quadTypes)
        {
            SourceAssetFile = sourceAssetFile;
            SourcePathId = sourcePathId;
            HasExplicitSource = true;
        }

        public ModelDefinition(
            string uniqueName,
            int uniqueIndex,
            string displayName,
            string description,
            string meshName,
            string sourceAssetFile,
            long sourcePathId,
            Quaternion visualRotation,
            float placementScale,
            bool centerVerticallyOnMesh,
            params RBlockQuadType[] quadTypes)
            : this(uniqueName, uniqueIndex, displayName, description, meshName, sourceAssetFile, sourcePathId, visualRotation, placementScale, centerVerticallyOnMesh, DefaultColliderAxisScale, quadTypes)
        {
        }
    }

    private class MeshVisualData
    {
        public readonly Mesh Mesh;
        public readonly Material[] Materials;
        public readonly GameObject CompositeTemplate;

        public MeshVisualData(Mesh mesh, Material[] materials, GameObject compositeTemplate)
        {
            Mesh = mesh;
            Materials = materials;
            CompositeTemplate = compositeTemplate;
        }
    }
}