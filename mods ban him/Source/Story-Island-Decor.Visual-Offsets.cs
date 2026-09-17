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

public partial class test_balloons
{   
    private static Vector3 GetVisualOffset(string uniqueName, Mesh mesh)
    {
        if (uniqueName == StairwellWallLampItemUniqueName)
        {
            return StairWallLampVisualOffset;
        }

        if (uniqueName == WallLampsItemUniqueName)
        {
            return WallLampVisualOffset;
        }

        if (uniqueName == SignDoorSurfaceAccessItemUniqueName)
        {
            return SurfaceAccessVisualOffset;
        }

        if (uniqueName == SignWallLoadingBayUpItemUniqueName)
        {
            return LoadingBayVisualOffset;
        }

        if (uniqueName == SignWallSurfaceAccessUpItemUniqueName || uniqueName == SignWallPlantationItemUniqueName)
        {
            return LoadingBayVisualOffset;
        }

        if (uniqueName == Toilet01ItemUniqueName)
        {
            return ToiletVisualOffset;
        }

        if (uniqueName == TangaroaBathroomSinkItemUniqueName && mesh != null)
        {
            Bounds bounds = mesh.bounds;
            return new Vector3(-bounds.center.x, -bounds.min.y, bounds.center.z);
        }

        if (uniqueName == Hanger01ItemUniqueName && mesh != null)
        {
            return new Vector3(Hanger01VisualOffset.x, -mesh.bounds.center.y, Hanger01VisualOffset.z);
        }

        if (uniqueName == TowelHangerItemUniqueName && mesh != null)
        {
            return new Vector3(TowelHangerVisualOffset.x, -mesh.bounds.center.y, TowelHangerVisualOffset.z);
        }

        if (uniqueName == VentItemUniqueName && mesh != null)
        {
            return new Vector3(VentVisualOffset.x, -mesh.bounds.center.y, VentVisualOffset.z);
        }

        if (uniqueName == SharedAssets6TableItemUniqueName && mesh != null)
        {
            return new Vector3(SharedAssets6TableVisualOffset.x, SharedAssets6TableVisualOffset.y, SharedAssets6TableVisualOffset.z);
        }

        if (uniqueName == RTCommRadioItemUniqueName && mesh != null)
        {
            return new Vector3(SharedAssets6TableVisualOffset.x, SharedAssets6TableVisualOffset.y, SharedAssets6TableVisualOffset.z);
        }

        if (uniqueName == ClipboardItemUniqueName && mesh != null)
        {
            return new Vector3(ClipboardVisualOffset.x, ClipboardVisualOffset.y, ClipboardVisualOffset.z);
        }

        if (uniqueName == RTPowerBoxItemUniqueName && mesh != null)
        {
            return RTPowerBoxVisualOffset;
        };

        if (uniqueName == RTWallVentItemUniqueName && mesh != null)
        {
            return RTWallVentVisualOffset;
        };

        ModelDefinition model;
        if (mesh != null && ModelsByUniqueName.TryGetValue(uniqueName, out model) && model.CenterVerticallyOnMesh)
        {
            return new Vector3(BalloonVisualOffset.x, -mesh.bounds.center.y, BalloonVisualOffset.z);
        }

        if (uniqueName == CustomItemUniqueName)
        {
            return BalloonVisualOffset;
        }

        if (uniqueName == HedgeBush1ItemUniqueName || uniqueName == HedgeBush2ItemUniqueName)
        {
            return BalloonVisualOffset;
        }

        if (uniqueName == WallBalloons1ItemUniqueName)
        {
            return BalloonVisualOffset;
        }

        if (uniqueName == VentItemUniqueName)
        {
            return BalloonVisualOffset;
        }

        // Exempt all signs from bottom-center placement
        if (uniqueName.Contains("Sign"))
        {
            return BalloonVisualOffset;
        }

        if (mesh != null)
        {
            Bounds bounds = mesh.bounds;
            // Bottom-center anchor
            return new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z + BalloonVisualOffset.z);
        }

        return BalloonVisualOffset;
    }
}