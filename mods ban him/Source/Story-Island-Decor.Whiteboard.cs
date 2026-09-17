using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public partial class test_balloons
{
    private const int CustomWhiteboardTextureSize = 2048;
    private const int CustomWhiteboardBrushRadius = 3;
    private static readonly Color CustomWhiteboardInkColor = new Color(0.08f, 0.08f, 0.08f, 1f);
    private static readonly Color CustomWhiteboardInkColorRed = new Color(0.73f, 0.10f, 0.10f, 1f);
    private static readonly Color CustomWhiteboardInkColorBlue = new Color(0.12f, 0.28f, 0.72f, 1f);
    private static readonly Color CustomWhiteboardInkColorGreen = new Color(0.12f, 0.50f, 0.22f, 1f);
    private static readonly string[] WhiteboardPenKeywords = { "Pen", "Marker" };
    private static readonly string[] WhiteboardEraserKeywords = { "Eraser", "Erasor", "Rubber" };
    private static readonly Vector2 WhiteboardToolSelectUVMin = new Vector2(0.73f, 0.43f);
    private static readonly Vector2 WhiteboardToolSelectUVMax = new Vector2(0.96f, 0.67f);
    private const float WhiteboardToolSelectTrayYOffset = 0.12f;
    private const float CustomWhiteboardMaxDrawDistance = 6f;
    private const float WhiteboardDusterFullClearHoldTime = 1.5f;
    private static readonly Vector3 CustomWhiteboardDrawAreaCenterLocal = Vector3.zero;
    private static readonly Vector2 CustomWhiteboardDrawAreaSize = new Vector2(4.50606f, 1.684126f);
    private static readonly Vector2 CustomWhiteboardDrawUVMin = new Vector2(0.0f, 0.0f);
    private static readonly Vector2 CustomWhiteboardDrawUVMax = new Vector2(0.918f, 0.347f);
    private const bool CustomWhiteboardFlipU = true;
    private const bool CustomWhiteboardFlipV = false;
    private const float CustomWhiteboardUVGainU = 0.985f;
    private const float CustomWhiteboardUVGainV = 0.985f;
    private const string WhiteboardSavePrefix = "WBPNG:";
    private static readonly Dictionary<uint, WhiteboardDrawingState> WhiteboardStatesByObjectIndex = new Dictionary<uint, WhiteboardDrawingState>();

    private struct WhiteboardToolSelectionDefinition
    {
        public string ChildName;
        public string ToolName;
        public string PromptText;
        public Vector3 LocalPosition;
        public Vector3 ColliderCenter;
        public Vector3 ColliderSize;
    }

    private static readonly WhiteboardToolSelectionDefinition[] WhiteboardToolSelectionDefinitions =
    {
        new WhiteboardToolSelectionDefinition
        {
            ChildName = "pen_red",
            ToolName = "PenRed",
            PromptText = "Use red pen",
            LocalPosition = new Vector3(1.092f, -0.608f, 0.239f),
            ColliderCenter = new Vector3(0.1253498f, -0.238393f, -0.1144467f),
            ColliderSize = new Vector3(0.3604466f, 0.03552473f, 0.1378608f)
        },
        new WhiteboardToolSelectionDefinition
        {
            ChildName = "pen_black",
            ToolName = "PenBlack",
            PromptText = "Use black pen",
            LocalPosition = new Vector3(0.0804f, -0.6059f, 0.2415f),
            ColliderCenter = new Vector3(0.1253498f, -0.238393f, -0.1144467f),
            ColliderSize = new Vector3(0.3604466f, 0.03552473f, 0.1378608f)
        },
        new WhiteboardToolSelectionDefinition
        {
            ChildName = "pen_blue",
            ToolName = "PenBlue",
            PromptText = "Use blue pen",
            LocalPosition = new Vector3(-0.3636f, -0.6059f, 0.2313f),
            ColliderCenter = new Vector3(0.1253498f, -0.238393f, -0.1144467f),
            ColliderSize = new Vector3(0.3604466f, 0.03552473f, 0.1378608f)
        },
        new WhiteboardToolSelectionDefinition
        {
            ChildName = "Duster",
            ToolName = "Eraser",
            PromptText = "Use eraser",
            LocalPosition = new Vector3(-1.6245f, -0.6f, 0.2269f),
            ColliderCenter = new Vector3(0.1399584f, -0.2247734f, -0.1087361f),
            ColliderSize = new Vector3(0.3896638f, 0.06276387f, 0.1411469f)
        }
    };

    private static void ConfigureWhiteboardDrawing(GameObject targetObject, string uniqueName)
    {
        if (targetObject == null || uniqueName != WhiteBoardItemUniqueName)
        {
            return;
        }

        if (!EnsureCustomWhiteboardDrawingSurface(targetObject))
        {
            Debug.LogWarning("Could not configure drawable whiteboard surface. The board will remain decorative.");
        }
    }

    private static bool EnsureCustomWhiteboardDrawingSurface(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return false;
        }

        MeshRenderer surfaceRenderer = FindWhiteboardSurfaceRenderer(targetObject);
        if (surfaceRenderer == null)
        {
            return false;
        }

        CustomWhiteboardDrawingSurface drawable = targetObject.GetComponent<CustomWhiteboardDrawingSurface>();
        if (drawable == null)
        {
            drawable = targetObject.AddComponent<CustomWhiteboardDrawingSurface>();
        }

        drawable.Initialize(surfaceRenderer, targetObject.transform);

        WhiteboardDrawingPersistenceID persistence = targetObject.GetComponent<WhiteboardDrawingPersistenceID>();
        if (persistence == null)
        {
            persistence = targetObject.AddComponent<WhiteboardDrawingPersistenceID>();
        }

        persistence.Initialize(targetObject.transform);
        return true;
    }

    private static void EnsureWhiteboardSurfaceCollider(MeshRenderer surfaceRenderer, Mesh surfaceMesh)
    {
        // If
    }

    private static MeshRenderer FindWhiteboardSurfaceRenderer(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return null;
        }

        Transform customVisual = targetObject.transform.Find("CustomVisual");
        if (customVisual != null)
        {
            MeshRenderer visualRenderer = customVisual.GetComponent<MeshRenderer>();
            if (visualRenderer != null)
            {
                return visualRenderer;
            }

            MeshRenderer childRenderer = customVisual.GetComponentInChildren<MeshRenderer>(true);
            if (childRenderer != null)
            {
                return childRenderer;
            }
        }

        return targetObject.GetComponentInChildren<MeshRenderer>(true);
    }

    private class CustomWhiteboardDrawingSurface : MonoBehaviour
    {
        private enum WhiteboardTool
        {
            PenBlack,
            PenRed,
            PenBlue,
            PenGreen,
            Eraser
        }

        private MeshRenderer surfaceRenderer;
        private Transform rootTransform;
        private Collider surfaceCollider;
        private WhiteboardDrawingState drawingState;
        private Texture baseTexture;
        private Texture2D drawingTexture;
        private Texture2D originalBoardTexture;
        private Color32[] originalBoardPixels;
        private Material runtimeMaterial;
        private Camera cachedCamera;
        private CanvasHelper cachedCanvasHelper;
        private DisplayTextManager cachedDisplayTextManager;
        private bool dirty;
        private float nextApplyTime;
        private bool hasLastDrawUV;
        private Vector2 lastDrawUV;
        private static WhiteboardTool sharedActiveTool = WhiteboardTool.PenBlack;
        private static Texture2D centerDotTexture;
        private string currentHoveredToolName;
        private float dusterClearHoldTimer;
        private bool showCenterColorDot;

        private class WhiteboardToolRaycastTarget : MonoBehaviour, IRaycastable
        {
            private static DisplayTextManager displayTextManager;
            private CustomWhiteboardDrawingSurface surface;
            private string toolName;
            private string promptText;

            public string PromptText
            {
                get { return promptText; }
            }

            public void Initialize(CustomWhiteboardDrawingSurface owner, string selectedToolName, string text)
            {
                surface = owner;
                toolName = selectedToolName;
                promptText = string.IsNullOrEmpty(text) ? "Press E to use tool" : text;
            }

            public void OnIsRayed()
            {
                if (displayTextManager == null)
                {
                    displayTextManager = ComponentManager<DisplayTextManager>.Value;
                }

                if (displayTextManager != null)
                {
                    displayTextManager.ShowText(promptText);
                }

                if (Input.GetKeyDown(KeyCode.E) && surface != null)
                {
                    surface.SetTool(toolName);
                }
            }

            public void OnRayEnter()
            {
                if (displayTextManager == null)
                {
                    displayTextManager = ComponentManager<DisplayTextManager>.Value;
                }
            }

            public void OnRayExit()
            {
                if (displayTextManager != null)
                {
                    displayTextManager.HideDisplayTexts();
                }
            }

            public void Activate()
            {
                if (surface != null)
                {
                    surface.SetTool(toolName);
                }
            }
        }

        public void Initialize(MeshRenderer renderer, Transform root)
        {
            if (renderer == null)
            {
                return;
            }

            surfaceRenderer = renderer;
            rootTransform = root != null ? root : transform;
            surfaceCollider = renderer.GetComponent<Collider>();
            drawingState = rootTransform.GetComponent<WhiteboardDrawingState>();
            if (drawingState == null)
            {
                drawingState = rootTransform.gameObject.AddComponent<WhiteboardDrawingState>();
            }
            baseTexture = ResolveSourceTexture(renderer);

            if (drawingTexture == null)
            {
                drawingTexture = new Texture2D(CustomWhiteboardTextureSize, CustomWhiteboardTextureSize, TextureFormat.RGBA32, false);
                drawingTexture.wrapMode = TextureWrapMode.Clamp;
                drawingTexture.filterMode = FilterMode.Bilinear;

                InitializeOriginalBoardTexture(baseTexture);

                if (!drawingState.TryLoadInto(drawingTexture))
                {
                    RestoreDrawingTextureToOriginal();
                }
            }

            if (originalBoardTexture == null)
            {
                InitializeOriginalBoardTexture(baseTexture);
            }

            drawingState.BindRuntimeTexture(drawingTexture);

            EnsureToolSelectionBounds();

            if (runtimeMaterial == null)
            {
                Material sourceMaterial = renderer.sharedMaterial;
                runtimeMaterial = sourceMaterial != null ? new Material(sourceMaterial) : new Material(Shader.Find("Standard"));
            }

            AssignTextureToMaterial(runtimeMaterial, drawingTexture);
            surfaceRenderer.sharedMaterial = runtimeMaterial;
            surfaceRenderer.SetPropertyBlock(null);
        }

        private static Texture ResolveSourceTexture(MeshRenderer renderer)
        {
            if (renderer == null)
            {
                return null;
            }

            Material sourceMaterial = renderer.sharedMaterial;
            if (sourceMaterial == null)
            {
                return null;
            }

            Texture texture = sourceMaterial.mainTexture;
            if (texture == null)
            {
                texture = sourceMaterial.GetTexture("_BaseMap");
            }

            return texture;
        }

        private void InitializeDrawingTexture(Texture sourceTexture)
        {
            Color fillColor = Color.white;
            if (sourceTexture == null)
            {
                FillTexture(drawingTexture, fillColor);
                return;
            }

            if (TryCopyTextureToReadableTexture(sourceTexture, drawingTexture))
            {
                return;
            }

            FillTexture(drawingTexture, fillColor);
        }

        private void InitializeOriginalBoardTexture(Texture sourceTexture)
        {
            if (originalBoardTexture == null)
            {
                originalBoardTexture = new Texture2D(CustomWhiteboardTextureSize, CustomWhiteboardTextureSize, TextureFormat.RGBA32, false);
                originalBoardTexture.wrapMode = TextureWrapMode.Clamp;
                originalBoardTexture.filterMode = FilterMode.Bilinear;
            }

            bool copied = sourceTexture != null && TryCopyTextureToReadableTexture(sourceTexture, originalBoardTexture);
            if (!copied)
            {
                FillTexture(originalBoardTexture, Color.white);
            }

            originalBoardPixels = originalBoardTexture.GetPixels32();
        }

        private void RestoreDrawingTextureToOriginal()
        {
            if (drawingTexture == null)
            {
                return;
            }

            if (originalBoardPixels == null || originalBoardPixels.Length != drawingTexture.width * drawingTexture.height)
            {
                InitializeOriginalBoardTexture(baseTexture);
            }

            if (originalBoardPixels != null && originalBoardPixels.Length == drawingTexture.width * drawingTexture.height)
            {
                drawingTexture.SetPixels32(originalBoardPixels);
                drawingTexture.Apply(false, false);
                return;
            }

            InitializeDrawingTexture(baseTexture);
        }

        private static bool TryCopyTextureToReadableTexture(Texture sourceTexture, Texture2D targetTexture)
        {
            if (sourceTexture == null || targetTexture == null)
            {
                return false;
            }

            RenderTexture renderTexture = null;
            RenderTexture previousActive = RenderTexture.active;
            try
            {
                renderTexture = RenderTexture.GetTemporary(targetTexture.width, targetTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                Graphics.Blit(sourceTexture, renderTexture);
                RenderTexture.active = renderTexture;
                targetTexture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
                targetTexture.Apply(false, false);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                RenderTexture.active = previousActive;
                if (renderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(renderTexture);
                }
            }
        }

        private static void FillTexture(Texture2D targetTexture, Color color)
        {
            if (targetTexture == null)
            {
                return;
            }

            Color[] pixels = new Color[targetTexture.width * targetTexture.height];
            for (int index = 0; index < pixels.Length; index++)
            {
                pixels[index] = color;
            }

            targetTexture.SetPixels(pixels);
            targetTexture.Apply(false, false);
        }

        private static void AssignTextureToMaterial(Material material, Texture texture)
        {
            if (material == null)
            {
                return;
            }

            material.mainTexture = texture;
            material.color = Color.white;
            if (material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", texture);
                material.SetColor("_BaseColor", Color.white);
            }
            if (material.HasProperty("_MainTex"))
            {
                material.SetTexture("_MainTex", texture);
                material.SetColor("_Color", Color.white);
            }
        }

        private void Update()
        {
            if (surfaceRenderer == null || drawingTexture == null)
            {
                return;
            }

            Camera activeCamera = GetActiveCamera();
            if (activeCamera == null)
            {
                return;
            }

            float useDistance = CustomWhiteboardMaxDrawDistance;
            try
            {
                useDistance = Mathf.Max(0.1f, Player.UseDistance);
            }
            catch
            {
                useDistance = CustomWhiteboardMaxDrawDistance;
            }

            UpdateCenterColorDot(activeCamera, useDistance);

            HandleToolSelectionInput(activeCamera, useDistance);

            if (!Input.GetMouseButton(1))
            {
                ApplyPendingPixels();
                if (hasLastDrawUV)
                {
                    PersistDrawingState();
                }
                hasLastDrawUV = false;
                return;
            }

            Vector3 mousePosition = Input.mousePosition;
            Ray ray = activeCamera.ScreenPointToRay(mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, useDistance * 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                hasLastDrawUV = false;
                ApplyPendingPixels();
                return;
            }

            if (!IsHitPartOfWhiteboard(hit.transform))
            {
                hasLastDrawUV = false;
                ApplyPendingPixels();
                return;
            }

            if (surfaceCollider != null && hit.collider != surfaceCollider)
            {
                if (!surfaceCollider.Raycast(ray, out hit, useDistance * 2f))
                {
                    hasLastDrawUV = false;
                    ApplyPendingPixels();
                    return;
                }
            }

            if (!TryResolveDrawUV(ray, hit, out Vector2 drawUV))
            {
                hasLastDrawUV = false;
                ApplyPendingPixels();
                return;
            }

            DrawAtUV(drawUV);
            ApplyPendingPixels();
        }

        private void OnGUI()
        {
            if (!showCenterColorDot)
            {
                return;
            }

            Texture2D dotTexture = GetCenterDotTexture();
            if (dotTexture == null)
            {
                return;
            }

            const float dotSize = 10f;
            Rect rect = new Rect((Screen.width - dotSize) * 0.5f, (Screen.height - dotSize) * 0.5f, dotSize, dotSize);
            Color previousColor = GUI.color;
            GUI.color = GetActiveToolColor();
            GUI.DrawTexture(rect, dotTexture);
            GUI.color = previousColor;
        }

        private void UpdateCenterColorDot(Camera activeCamera, float useDistance)
        {
            showCenterColorDot = false;

            if (activeCamera == null || CanvasHelper.ActiveMenu != MenuType.None)
            {
                return;
            }

            Ray ray = activeCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            if (!Physics.Raycast(ray, out RaycastHit hit, useDistance * 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            if (!IsHitPartOfWhiteboard(hit.transform))
            {
                return;
            }

            if (surfaceCollider != null && hit.collider != surfaceCollider)
            {
                if (!surfaceCollider.Raycast(ray, out hit, useDistance * 2f))
                {
                    return;
                }
            }

            if (!TryResolveDrawUV(ray, hit, out Vector2 _))
            {
                return;
            }

            showCenterColorDot = true;
        }

        private static Texture2D GetCenterDotTexture()
        {
            if (centerDotTexture != null)
            {
                return centerDotTexture;
            }

            const int size = 64;
            centerDotTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            centerDotTexture.wrapMode = TextureWrapMode.Clamp;
            centerDotTexture.filterMode = FilterMode.Bilinear;

            float radius = (size - 1) * 0.5f;
            float radiusSquared = radius * radius;
            Color clear = new Color(1f, 1f, 1f, 0f);
            Color solid = Color.white;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float distSquared = dx * dx + dy * dy;
                    centerDotTexture.SetPixel(x, y, distSquared <= radiusSquared ? solid : clear);
                }
            }

            centerDotTexture.Apply(false, true);
            return centerDotTexture;
        }

        private void HandleToolSelectionInput(Camera activeCamera, float useDistance)
        {
            if (activeCamera == null)
            {
                ClearHoveredToolPrompt();
                return;
            }

            if (CanvasHelper.ActiveMenu != MenuType.None)
            {
                ClearHoveredToolPrompt();
                return;
            }

            Ray ray = activeCamera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
            float interactDistance = Mathf.Max(1f, useDistance * 2f);
            int interactableMask = (int)LayerMasks.MASK_RaycastInteractable;
            RaycastHit[] hits = interactableMask != 0
                ? Physics.RaycastAll(ray, interactDistance, interactableMask, QueryTriggerInteraction.Collide)
                : Physics.RaycastAll(ray, interactDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            if (hits == null || hits.Length == 0)
            {
                ClearHoveredToolPrompt();
                return;
            }

            string toolName;
            string promptText;
            if (!TryResolveToolFromHits(hits, out toolName, out promptText))
            {
                ClearHoveredToolPrompt();
                return;
            }

            currentHoveredToolName = toolName;
            DisplayToolPrompt(promptText);

            if (Input.GetKeyDown(KeyCode.E))
            {
                SetTool(toolName);
            }

            HandleDusterFullClear(toolName);
        }

        private void HandleDusterFullClear(string toolName)
        {
            bool isDuster = string.Equals(toolName, "Eraser", StringComparison.OrdinalIgnoreCase);
            if (!isDuster)
            {
                ResetDusterHoldToClear();
                return;
            }

            DisplayDusterHoldPrompt();

            if (IsRemovePressed())
            {
                dusterClearHoldTimer += Time.deltaTime;
                CanvasHelper canvas = GetCanvasHelper();
                if (canvas != null)
                {
                    canvas.SetLoadCircle(true);
                    canvas.SetLoadCircle(dusterClearHoldTimer / WhiteboardDusterFullClearHoldTime);
                }

                if (dusterClearHoldTimer >= WhiteboardDusterFullClearHoldTime)
                {
                    dusterClearHoldTimer = 0f;
                    ClearBoardCompletely();
                    if (canvas != null)
                    {
                        canvas.SetLoadCircle(false);
                    }
                }
            }
            else if (IsRemoveReleased())
            {
                ResetDusterHoldToClear();
            }
        }

        private void DisplayDusterHoldPrompt()
        {
            if (cachedDisplayTextManager == null)
            {
                cachedDisplayTextManager = ComponentManager<DisplayTextManager>.Value;
            }

            if (cachedDisplayTextManager == null)
            {
                return;
            }

            cachedDisplayTextManager.ShowText("Hold to clear", KeyCode.C, 3, clearAllTexts: false);
        }

        private bool TryResolveToolFromHits(RaycastHit[] hits, out string toolName, out string promptText)
        {
            toolName = null;
            promptText = null;

            float nearestDistance = float.MaxValue;
            bool found = false;

            for (int index = 0; index < hits.Length; index++)
            {
                RaycastHit hit = hits[index];
                if (hit.collider == null || hit.distance >= nearestDistance)
                {
                    continue;
                }

                if (!TryResolveToolFromTransform(hit.collider.transform, out string hitToolName, out string hitPromptText))
                {
                    continue;
                }

                nearestDistance = hit.distance;
                toolName = hitToolName;
                promptText = hitPromptText;
                found = true;
            }

            return found;
        }

        private bool TryResolveToolFromTransform(Transform transform, out string toolName, out string promptText)
        {
            toolName = null;
            promptText = null;

            if (transform == null || rootTransform == null)
            {
                return false;
            }

            if (transform != rootTransform && !transform.IsChildOf(rootTransform))
            {
                return false;
            }

            Transform current = transform;
            while (current != null)
            {
                if (current == rootTransform)
                {
                    break;
                }

                for (int index = 0; index < WhiteboardToolSelectionDefinitions.Length; index++)
                {
                    WhiteboardToolSelectionDefinition definition = WhiteboardToolSelectionDefinitions[index];
                    if (!string.Equals(current.name, definition.ChildName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    toolName = definition.ToolName;
                    promptText = definition.PromptText;
                    return true;
                }

                current = current.parent;
            }

            return false;
        }

        private void DisplayToolPrompt(string promptText)
        {
            if (cachedDisplayTextManager == null)
            {
                cachedDisplayTextManager = ComponentManager<DisplayTextManager>.Value;
            }

            if (cachedDisplayTextManager == null)
            {
                return;
            }

            string text = string.IsNullOrEmpty(promptText) ? Helper.GetTerm("Game/Use") : promptText;
            string interactBinding = GetBindingDisplayString("Interact");
            KeyCode interactKey = GetKeybindMainKey("Interact");
            cachedDisplayTextManager.ShowText(text, interactBinding, interactKey, 0, clearAllTexts: true);
        }

        private void ClearHoveredToolPrompt()
        {
            if (string.IsNullOrEmpty(currentHoveredToolName))
            {
                return;
            }

            if (cachedDisplayTextManager == null)
            {
                cachedDisplayTextManager = ComponentManager<DisplayTextManager>.Value;
            }

            if (cachedDisplayTextManager != null)
            {
                CanvasHelper canvas = GetCanvasHelper();
                if (canvas != null)
                {
                    canvas.SetLoadCircle(false);
                }
                cachedDisplayTextManager.HideDisplayTexts(0);
                cachedDisplayTextManager.HideDisplayTexts(3);
            }

            currentHoveredToolName = null;
            dusterClearHoldTimer = 0f;
        }

        private void ResetDusterHoldToClear()
        {
            dusterClearHoldTimer = 0f;
            CanvasHelper canvas = GetCanvasHelper();
            if (canvas != null)
            {
                canvas.SetLoadCircle(false);
            }

            if (cachedDisplayTextManager == null)
            {
                cachedDisplayTextManager = ComponentManager<DisplayTextManager>.Value;
            }

            if (cachedDisplayTextManager != null)
            {
                cachedDisplayTextManager.HideDisplayTexts(3);
            }
        }

        private CanvasHelper GetCanvasHelper()
        {
            if (cachedCanvasHelper == null)
            {
                cachedCanvasHelper = ComponentManager<CanvasHelper>.Value;
            }

            return cachedCanvasHelper;
        }

        private static string GetBindingDisplayString(string actionName)
        {
            try
            {
                PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);
                if (playerInput != null)
                {
                    InputAction action = playerInput.actions[actionName];
                    if (action != null)
                    {
                        return action.GetBindingDisplayString();
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        private static KeyCode GetKeybindMainKey(string keybindName)
        {
            try
            {
                return MyInput.Keybinds[keybindName].MainKey;
            }
            catch
            {
                return KeyCode.None;
            }
        }

        private static bool IsRemovePressed()
        {
            return Input.GetKey(KeyCode.C);
        }

        private static bool IsRemoveReleased()
        {
            return Input.GetKeyUp(KeyCode.C);
        }

        private void ClearBoardCompletely()
        {
            RestoreDrawingTextureToOriginal();
            hasLastDrawUV = false;
            dirty = false;
            PersistDrawingState();
        }

        private void EnsureToolSelectionBounds()
        {
            if (rootTransform == null)
            {
                return;
            }

            int interactableLayer = ResolveInteractableLayer(rootTransform);

            for (int index = 0; index < WhiteboardToolSelectionDefinitions.Length; index++)
            {
                WhiteboardToolSelectionDefinition definition = WhiteboardToolSelectionDefinitions[index];
                Transform toolTransform = FindOrCreateToolSelectionTransform(rootTransform, definition);
                if (toolTransform == null)
                {
                    continue;
                }

                toolTransform.gameObject.layer = interactableLayer;

                BoxCollider selectionCollider = toolTransform.GetComponent<BoxCollider>();
                if (selectionCollider == null)
                {
                    selectionCollider = toolTransform.gameObject.AddComponent<BoxCollider>();
                }

                selectionCollider.center = definition.ColliderCenter;
                selectionCollider.size = definition.ColliderSize;
                selectionCollider.isTrigger = false;
                selectionCollider.enabled = true;

                InteractableButton existingButton = toolTransform.GetComponent<InteractableButton>();
                if (existingButton != null)
                {
                    Destroy(existingButton);
                }

                RaycastInteractable raycastInteractable = toolTransform.GetComponent<RaycastInteractable>();
                if (raycastInteractable == null)
                {
                    raycastInteractable = toolTransform.gameObject.AddComponent<RaycastInteractable>();
                }

                WhiteboardToolRaycastTarget toolRaycastTarget = toolTransform.GetComponent<WhiteboardToolRaycastTarget>();
                if (toolRaycastTarget == null)
                {
                    toolRaycastTarget = toolTransform.gameObject.AddComponent<WhiteboardToolRaycastTarget>();
                }

                toolRaycastTarget.Initialize(this, definition.ToolName, definition.PromptText);
                raycastInteractable.AddRaycastables(new IRaycastable[] { toolRaycastTarget });
            }
        }

        private static int ResolveInteractableLayer(Transform root)
        {
            if (root == null)
            {
                return 0;
            }

            int layerFromMask = FirstEnabledLayerIndex((int)LayerMasks.MASK_RaycastInteractable);
            if (layerFromMask >= 0)
            {
                return layerFromMask;
            }

            RaycastInteractable existingInteractable = root.GetComponentInChildren<RaycastInteractable>(true);
            if (existingInteractable != null)
            {
                return existingInteractable.gameObject.layer;
            }

            int raycastInteractableLayer = LayerMask.NameToLayer("RaycastInteractable");
            if (raycastInteractableLayer >= 0)
            {
                return raycastInteractableLayer;
            }

            raycastInteractableLayer = LayerMask.NameToLayer("Raycast Interactable");
            if (raycastInteractableLayer >= 0)
            {
                return raycastInteractableLayer;
            }

            return root.gameObject.layer;
        }

        private static int FirstEnabledLayerIndex(int layerMask)
        {
            if (layerMask == 0)
            {
                return -1;
            }

            for (int index = 0; index < 32; index++)
            {
                if ((layerMask & (1 << index)) != 0)
                {
                    return index;
                }
            }

            return -1;
        }

        private static Transform FindOrCreateToolSelectionTransform(Transform root, WhiteboardToolSelectionDefinition definition)
        {
            if (root == null)
            {
                return null;
            }

            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            Transform fallback = null;
            for (int index = 0; index < children.Length; index++)
            {
                Transform child = children[index];
                if (child == null || !string.Equals(child.name, definition.ChildName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (child.GetComponent<Collider>() != null)
                {
                    return child;
                }

                if (fallback == null)
                {
                    fallback = child;
                }
            }

            if (fallback != null)
            {
                return fallback;
            }

            GameObject toolObject = new GameObject(definition.ChildName);
            toolObject.transform.SetParent(root, false);
            toolObject.transform.localPosition = definition.LocalPosition;
            toolObject.transform.localRotation = Quaternion.identity;
            toolObject.transform.localScale = Vector3.one;
            return toolObject.transform;
        }

        public void SetTool(string toolName)
        {
            sharedActiveTool = ResolveToolFromName(toolName);
            hasLastDrawUV = false;
        }

        private static WhiteboardTool ResolveToolFromName(string toolName)
        {
            if (string.IsNullOrEmpty(toolName))
            {
                return WhiteboardTool.PenBlack;
            }

            if (toolName.IndexOf("Red", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return WhiteboardTool.PenRed;
            }

            if (toolName.IndexOf("Blue", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return WhiteboardTool.PenBlue;
            }

            if (toolName.IndexOf("Green", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return WhiteboardTool.PenGreen;
            }

            if (toolName.IndexOf("Eraser", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return WhiteboardTool.Eraser;
            }

            return WhiteboardTool.PenBlack;
        }

        private bool TryResolveDrawUV(Ray ray, RaycastHit hit, out Vector2 uv)
        {
            uv = Vector2.zero;

            if (surfaceRenderer == null)
            {
                return false;
            }

            Transform surfaceTransform = surfaceRenderer.transform;
            Vector3 localPoint = surfaceTransform.InverseTransformPoint(hit.point);

            Vector3 rayOriginLocal = surfaceTransform.InverseTransformPoint(ray.origin);
            Vector3 rayDirectionLocal = surfaceTransform.InverseTransformDirection(ray.direction);
            if (rayDirectionLocal.sqrMagnitude > 0.000001f)
            {
                rayDirectionLocal.Normalize();
                float denom = rayDirectionLocal.z;
                if (Mathf.Abs(denom) > 0.0001f)
                {
                    float t = (CustomWhiteboardDrawAreaCenterLocal.z - rayOriginLocal.z) / denom;
                    if (t >= 0f)
                    {
                        localPoint = rayOriginLocal + rayDirectionLocal * t;
                    }
                }
            }

            return TryResolveDrawUVFromLocalPoint(surfaceTransform, localPoint, out uv);
        }

        private static bool TryResolveDrawUVFromLocalPoint(Transform surfaceTransform, Vector3 localPoint, out Vector2 uv)
        {
            uv = Vector2.zero;

            Vector3 center = CustomWhiteboardDrawAreaCenterLocal;
            Vector3 lossyScale = surfaceTransform != null ? surfaceTransform.lossyScale : Vector3.one;
            float scaleX = Mathf.Max(0.0001f, Mathf.Abs(lossyScale.x));
            float scaleY = Mathf.Max(0.0001f, Mathf.Abs(lossyScale.y));
            float width = Mathf.Max(0.001f, CustomWhiteboardDrawAreaSize.x / scaleX);
            float height = Mathf.Max(0.001f, CustomWhiteboardDrawAreaSize.y / scaleY);

            float minX = center.x - width * 0.5f;
            float maxX = center.x + width * 0.5f;
            float minY = center.y - height * 0.5f;
            float maxY = center.y + height * 0.5f;

            float u = Mathf.InverseLerp(minX, maxX, localPoint.x);
            float v = Mathf.InverseLerp(minY, maxY, localPoint.y);

            u = Mathf.Clamp01(u);
            v = Mathf.Clamp01(v);

            if (CustomWhiteboardFlipU)
            {
                u = 1f - u;
            }

            if (CustomWhiteboardFlipV)
            {
                v = 1f - v;
            }

            u = 0.5f + (u - 0.5f) * CustomWhiteboardUVGainU;
            v = 0.5f + (v - 0.5f) * CustomWhiteboardUVGainV;
            u = Mathf.Clamp01(u);
            v = Mathf.Clamp01(v);

            float atlasMinU = Mathf.Clamp01(Mathf.Min(CustomWhiteboardDrawUVMin.x, CustomWhiteboardDrawUVMax.x));
            float atlasMaxU = Mathf.Clamp01(Mathf.Max(CustomWhiteboardDrawUVMin.x, CustomWhiteboardDrawUVMax.x));
            float atlasMinV = Mathf.Clamp01(Mathf.Min(CustomWhiteboardDrawUVMin.y, CustomWhiteboardDrawUVMax.y));
            float atlasMaxV = Mathf.Clamp01(Mathf.Max(CustomWhiteboardDrawUVMin.y, CustomWhiteboardDrawUVMax.y));
            u = Mathf.Lerp(atlasMinU, atlasMaxU, u);
            v = Mathf.Lerp(atlasMinV, atlasMaxV, v);

            uv = new Vector2(u, v);
            return true;
        }

        private Camera GetActiveCamera()
        {
            if (cachedCamera != null)
            {
                return cachedCamera;
            }

            cachedCamera = Camera.main;
            if (cachedCamera != null)
            {
                return cachedCamera;
            }

            Camera[] cameras = Object.FindObjectsOfType<Camera>();
            for (int index = 0; index < cameras.Length; index++)
            {
                if (cameras[index] != null && cameras[index].enabled)
                {
                    cachedCamera = cameras[index];
                    return cachedCamera;
                }
            }

            return null;
        }

        private bool IsHitPartOfWhiteboard(Transform hitTransform)
        {
            if (hitTransform == null)
            {
                return false;
            }

            return hitTransform == rootTransform || hitTransform.IsChildOf(rootTransform);
        }

        private void DrawAtUV(Vector2 uv)
        {
            if (!hasLastDrawUV)
            {
                DrawStampAtUV(uv);
                lastDrawUV = uv;
                hasLastDrawUV = true;
                return;
            }

            Vector2 textureSize = new Vector2(drawingTexture.width - 1f, drawingTexture.height - 1f);
            Vector2 previousPixel = new Vector2(lastDrawUV.x * textureSize.x, lastDrawUV.y * textureSize.y);
            Vector2 currentPixel = new Vector2(uv.x * textureSize.x, uv.y * textureSize.y);

            float distancePixels = Vector2.Distance(previousPixel, currentPixel);
            float stampSpacingPixels = Mathf.Max(1f, CustomWhiteboardBrushRadius * 0.5f);
            int steps = Mathf.Max(1, Mathf.CeilToInt(distancePixels / stampSpacingPixels));

            for (int step = 1; step <= steps; step++)
            {
                float t = step / (float)steps;
                Vector2 interpolatedUV = Vector2.Lerp(lastDrawUV, uv, t);
                DrawStampAtUV(interpolatedUV);
            }

            lastDrawUV = uv;
            hasLastDrawUV = true;
        }

        private void DrawStampAtUV(Vector2 uv)
        {
            int xCenter = Mathf.RoundToInt(uv.x * (drawingTexture.width - 1));
            int yCenter = Mathf.RoundToInt(uv.y * (drawingTexture.height - 1));
            Color drawColor = GetActiveToolColor();
            bool useEraser = sharedActiveTool == WhiteboardTool.Eraser;
            bool hasOriginalSource = originalBoardPixels != null && originalBoardPixels.Length == drawingTexture.width * drawingTexture.height;

            int radius = useEraser ? CustomWhiteboardBrushRadius * 4 : CustomWhiteboardBrushRadius;
            int radiusSquared = radius * radius;
            for (int y = -radius; y <= radius; y++)
            {
                int yPos = yCenter + y;
                if (yPos < 0 || yPos >= drawingTexture.height)
                {
                    continue;
                }

                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y > radiusSquared)
                    {
                        continue;
                    }

                    int xPos = xCenter + x;
                    if (xPos < 0 || xPos >= drawingTexture.width)
                    {
                        continue;
                    }

                    if (useEraser && hasOriginalSource)
                    {
                        int pixelIndex = yPos * drawingTexture.width + xPos;
                        drawingTexture.SetPixel(xPos, yPos, originalBoardPixels[pixelIndex]);
                    }
                    else if (useEraser)
                    {
                        drawingTexture.SetPixel(xPos, yPos, Color.white);
                    }
                    else
                    {
                        drawingTexture.SetPixel(xPos, yPos, drawColor);
                    }

                    dirty = true;
                }
            }
        }

        private Color GetActiveToolColor()
        {
            switch (sharedActiveTool)
            {
                case WhiteboardTool.PenRed:
                    return CustomWhiteboardInkColorRed;
                case WhiteboardTool.PenBlue:
                    return CustomWhiteboardInkColorBlue;
                case WhiteboardTool.PenGreen:
                    return CustomWhiteboardInkColorGreen;
                default:
                    return CustomWhiteboardInkColor;
            }
        }

        private void ApplyPendingPixels()
        {
            if (!dirty || Time.time < nextApplyTime)
            {
                return;
            }

            drawingTexture.Apply(false, false);
            dirty = false;
            nextApplyTime = Time.time + 0.03f;
        }

        private void PersistDrawingState()
        {
            if (drawingState == null || drawingTexture == null)
            {
                return;
            }

            drawingState.SaveFrom(drawingTexture);
        }

        private void OnDisable()
        {
            ClearHoveredToolPrompt();
            ApplyPendingPixels();
            PersistDrawingState();
        }

        private void OnDestroy()
        {
            ClearHoveredToolPrompt();
            ApplyPendingPixels();
            PersistDrawingState();

            if (runtimeMaterial != null)
            {
                Object.Destroy(runtimeMaterial);
                runtimeMaterial = null;
            }

            if (drawingTexture != null)
            {
                Object.Destroy(drawingTexture);
                drawingTexture = null;
            }

            if (originalBoardTexture != null)
            {
                Object.Destroy(originalBoardTexture);
                originalBoardTexture = null;
                originalBoardPixels = null;
            }
        }
    }

    private class WhiteboardDrawingState : MonoBehaviour
    {
        [SerializeField]
        private int textureWidth;

        [SerializeField]
        private int textureHeight;

        [SerializeField]
        private byte[] rawTextureData;

        [SerializeField]
        private string encodedDrawingData;

        private uint registeredObjectIndex;
        private Texture2D runtimeTexture;
        private bool encodedDirty;
        private bool hasAnyDrawing;

        public bool HasSavedData
        {
            get
            {
                return hasAnyDrawing && ((rawTextureData != null && rawTextureData.Length > 0) || !string.IsNullOrEmpty(encodedDrawingData) || runtimeTexture != null);
            }
        }

        public void BindRuntimeTexture(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            runtimeTexture = texture;
            RegisterSelf();
        }

        public void SaveFrom(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }

            textureWidth = texture.width;
            textureHeight = texture.height;
            runtimeTexture = texture;
            hasAnyDrawing = true;
            encodedDirty = true;
            rawTextureData = null;
            RegisterSelf();
        }

        public bool TryLoadInto(Texture2D texture)
        {
            if (texture == null)
            {
                return false;
            }

            if (!TryLoadEncodedDataIntoTexture(texture, encodedDrawingData))
            {
                return false;
            }

            textureWidth = texture.width;
            textureHeight = texture.height;
            runtimeTexture = texture;
            hasAnyDrawing = true;
            encodedDirty = false;
            RegisterSelf();
            return true;
        }

        public string ExportEncodedData()
        {
            RegisterSelf();
            if (!hasAnyDrawing)
            {
                return string.Empty;
            }

            if (!encodedDirty && !string.IsNullOrEmpty(encodedDrawingData))
            {
                return encodedDrawingData;
            }

            if (runtimeTexture != null)
            {
                encodedDrawingData = EncodeTextureToBase64Png(runtimeTexture);
                encodedDirty = false;
            }

            return encodedDrawingData;
        }

        public bool ImportEncodedData(string payload)
        {
            if (string.IsNullOrEmpty(payload))
            {
                return false;
            }

            encodedDrawingData = payload;
            hasAnyDrawing = true;
            if (runtimeTexture != null)
            {
                if (!TryLoadEncodedDataIntoTexture(runtimeTexture, payload))
                {
                    return false;
                }

                textureWidth = runtimeTexture.width;
                textureHeight = runtimeTexture.height;
            }
            else if (!TryDecodeEncodedDataToRaw())
            {
                return false;
            }

            encodedDirty = false;

            RegisterSelf();
            return true;
        }

        private static bool TryLoadEncodedDataIntoTexture(Texture2D texture, string payload)
        {
            if (texture == null || string.IsNullOrEmpty(payload))
            {
                return false;
            }

            byte[] pngBytes;
            try
            {
                pngBytes = Convert.FromBase64String(payload);
            }
            catch
            {
                return false;
            }

            try
            {
                return texture.LoadImage(pngBytes, false);
            }
            catch
            {
                return false;
            }
        }

        private bool TryDecodeEncodedDataToRaw()
        {
            if (string.IsNullOrEmpty(encodedDrawingData))
            {
                return false;
            }

            byte[] pngBytes;
            try
            {
                pngBytes = Convert.FromBase64String(encodedDrawingData);
            }
            catch
            {
                return false;
            }

            Texture2D tempTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            bool loaded;
            try
            {
                loaded = tempTexture.LoadImage(pngBytes, false);
            }
            catch
            {
                loaded = false;
            }

            if (!loaded)
            {
                Object.Destroy(tempTexture);
                return false;
            }

            Color32[] sourcePixels = tempTexture.GetPixels32();
            Texture2D normalized = new Texture2D(tempTexture.width, tempTexture.height, TextureFormat.RGBA32, false);
            normalized.SetPixels32(sourcePixels);
            normalized.Apply(false, false);

            textureWidth = normalized.width;
            textureHeight = normalized.height;
            byte[] source = normalized.GetRawTextureData();
            rawTextureData = new byte[source.Length];
            Buffer.BlockCopy(source, 0, rawTextureData, 0, source.Length);
            hasAnyDrawing = true;
            Object.Destroy(normalized);
            Object.Destroy(tempTexture);
            return true;
        }

        private static string EncodeTextureToBase64Png(Texture2D texture)
        {
            if (texture == null)
            {
                return string.Empty;
            }

            byte[] pngBytes;
            try
            {
                pngBytes = texture.EncodeToPNG();
            }
            catch
            {
                return string.Empty;
            }

            return pngBytes != null && pngBytes.Length > 0 ? Convert.ToBase64String(pngBytes) : string.Empty;
        }

        private void RegisterSelf()
        {
            Block block = GetComponent<Block>();
            uint objectIndex = block != null ? block.ObjectIndex : 0U;
            if (objectIndex == 0U)
            {
                return;
            }

            if (registeredObjectIndex != 0U && registeredObjectIndex != objectIndex)
            {
                WhiteboardStatesByObjectIndex.Remove(registeredObjectIndex);
            }

            registeredObjectIndex = objectIndex;
            WhiteboardStatesByObjectIndex[registeredObjectIndex] = this;
        }

        private void OnEnable()
        {
            RegisterSelf();
        }

        private void LateUpdate()
        {
            if (registeredObjectIndex == 0U)
            {
                RegisterSelf();
            }
        }

        private void OnDisable()
        {
            if (registeredObjectIndex != 0U)
            {
                WhiteboardStatesByObjectIndex.Remove(registeredObjectIndex);
            }
        }

        private void OnDestroy()
        {
            if (registeredObjectIndex != 0U)
            {
                WhiteboardStatesByObjectIndex.Remove(registeredObjectIndex);
            }
        }
    }

    private class WhiteboardDrawingPersistenceID : MonoBehaviour_ID
    {
        private Transform rootTransform;
        private WhiteboardDrawingState drawingState;

        public void Initialize(Transform root)
        {
            rootTransform = root != null ? root : transform;
            drawingState = rootTransform.GetComponent<WhiteboardDrawingState>();
            if (drawingState == null)
            {
                drawingState = rootTransform.gameObject.AddComponent<WhiteboardDrawingState>();
            }
        }

        public override List<RGD> Serialize_SaveMultiple()
        {
            if (drawingState == null)
            {
                drawingState = (rootTransform != null ? rootTransform : transform).GetComponent<WhiteboardDrawingState>();
            }

            if (drawingState == null || !drawingState.HasSavedData)
            {
                return null;
            }

            Block block = (rootTransform != null ? rootTransform : transform).GetComponent<Block>();
            if (block == null || block.ObjectIndex == 0U || !block.IsValidSerialization())
            {
                return null;
            }

            string payload = drawingState.ExportEncodedData();
            if (string.IsNullOrEmpty(payload))
            {
                return null;
            }

            RGD_TextWriterObject rgd = new RGD_TextWriterObject();
            rgd.Type = RGDType.ID_TextWriterObject;
            rgd.SortingOrder = RGDSortingOrder.TextWriterObjects;
            rgd.connectedObjectIndex = block.ObjectIndex;
            rgd.text = WhiteboardSavePrefix + payload;

            return new List<RGD> { rgd };
        }
    }

    [HarmonyPatch(typeof(Block), "SetInstanceColorAndPattern")]
    private static class Patch_Block_SetInstanceColorAndPattern_CustomDecorGuard
    {
        private static bool Prefix(Block __instance)
        {
            if (__instance == null || __instance.buildableItem == null)
            {
                return true;
            }

            if (!CustomUniqueNames.Contains(__instance.buildableItem.UniqueName))
            {
                return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(SaveAndLoad), "LoadTextWriterLate")]
    private static class Patch_SaveAndLoad_LoadTextWriterLate_WhiteboardRestore
    {
        private static bool Prefix(RGD_TextWriterObject rgdTextWriter, float delay, ref System.Collections.IEnumerator __result)
        {
            if (rgdTextWriter == null || string.IsNullOrEmpty(rgdTextWriter.text) || !rgdTextWriter.text.StartsWith(WhiteboardSavePrefix, StringComparison.Ordinal))
            {
                return true;
            }

            __result = RestoreWhiteboardLate(rgdTextWriter, delay);
            return false;
        }

        private static System.Collections.IEnumerator RestoreWhiteboardLate(RGD_TextWriterObject rgdTextWriter, float delay)
        {
            yield return new WaitForSeconds(delay);

            string payload = rgdTextWriter.text.Substring(WhiteboardSavePrefix.Length);
            if (string.IsNullOrEmpty(payload))
            {
                yield break;
            }

            if (!WhiteboardStatesByObjectIndex.TryGetValue(rgdTextWriter.connectedObjectIndex, out WhiteboardDrawingState state) || state == null)
            {
                List<Block> placedBlocks = BlockCreator.GetPlacedBlocks();
                for (int index = 0; index < placedBlocks.Count; index++)
                {
                    Block block = placedBlocks[index];
                    if (block == null || block.ObjectIndex != rgdTextWriter.connectedObjectIndex)
                    {
                        continue;
                    }

                    state = block.GetComponent<WhiteboardDrawingState>();
                    if (state == null)
                    {
                        state = block.gameObject.AddComponent<WhiteboardDrawingState>();
                    }

                    break;
                }
            }

            if (state != null)
            {
                state.ImportEncodedData(payload);
            }
        }
    }
}
