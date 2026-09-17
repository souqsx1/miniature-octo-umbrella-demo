using UnityEngine;
using System.Collections.Generic;

public class SpecialItemCollider : MonoBehaviour
{
}

public class ConfiguredColliders : MonoBehaviour
{
}

public enum ColliderType { Box, Capsule }
public enum CapsuleDirection { X = 0, Y = 1, Z = 2 }

public struct ColliderDefinition
{
    public string Name;
    public Vector3 LocalPosition;
    public Vector3 LocalEulerRotation;
    public ColliderType Type;
    
    // BoxCollider properties
    public Vector3 ColliderCenter;
    public Vector3 ColliderSize;
    
    // CapsuleCollider properties
    public float CapsuleRadius;
    public float CapsuleHeight;
    public CapsuleDirection Direction;

    public ColliderDefinition(string name, Vector3 position, Vector3 center, Vector3 size)
    {
        Name = name;
        LocalPosition = position;
        LocalEulerRotation = Vector3.zero;
        Type = ColliderType.Box;
        ColliderCenter = center;
        ColliderSize = size;
        CapsuleRadius = 0f;
        CapsuleHeight = 0f;
        Direction = CapsuleDirection.X;
    }

    public ColliderDefinition(string name, Vector3 position, Vector3 rotationEuler, Vector3 center, Vector3 size)
    {
        Name = name;
        LocalPosition = position;
        LocalEulerRotation = rotationEuler;
        Type = ColliderType.Box;
        ColliderCenter = center;
        ColliderSize = size;
        CapsuleRadius = 0f;
        CapsuleHeight = 0f;
        Direction = CapsuleDirection.X;
    }

    public ColliderDefinition(string name, Vector3 position, Vector3 center, float radius, float height, CapsuleDirection direction)
    {
        Name = name;
        LocalPosition = position;
        LocalEulerRotation = Vector3.zero;
        Type = ColliderType.Capsule;
        ColliderCenter = center;
        ColliderSize = Vector3.zero;
        CapsuleRadius = radius;
        CapsuleHeight = height;
        Direction = direction;
    }

    public ColliderDefinition(string name, Vector3 position, Vector3 rotationEuler, Vector3 center, float radius, float height, CapsuleDirection direction)
    {
        Name = name;
        LocalPosition = position;
        LocalEulerRotation = rotationEuler;
        Type = ColliderType.Capsule;
        ColliderCenter = center;
        ColliderSize = Vector3.zero;
        CapsuleRadius = radius;
        CapsuleHeight = height;
        Direction = direction;
    }
}

// Defines all colliders for a specific item
public class ItemColliderSet
{
    public string ItemUniqueName;
    public ColliderDefinition[] Colliders;

    public ItemColliderSet(string itemUniqueName, params ColliderDefinition[] colliders)
    {
        ItemUniqueName = itemUniqueName;
        Colliders = colliders;
    }
}

// Manages special collider configurations for items
public static class ColliderManager
{
    // Define all item-specific collider sets here
    private static readonly ItemColliderSet[] ColliderSets = new ItemColliderSet[]
    {
        // Placeable_SunshadeGround - Umbrella item
        new ItemColliderSet("Placeable_SunshadeGround",
            new ColliderDefinition("Collider_1",
                new Vector3(0f, 0.14f, -0.003f),   // Transform Position
                new Vector3(-0.006367139f, -0.01967141f, 0.001177532f), // Box Collider Center
                new Vector3(0.6972317f, 0.211460f, 0.697645f)),    // Box Collider Size
            new ColliderDefinition("Collider_2",
                new Vector3(0.351f, -0.01f, -0.085f),   // Transform Position
                new Vector3(-0.3392765f, 1.800738f, 0.0861461f),    // Box Collider Center
                new Vector3(0.1521833f, 3.185422f, 0.1531721f)),    // Box Collider Size
            new ColliderDefinition("Collider_3",
                new Vector3(0.379f, 2.796f, 1.13f), // Transform Position
                new Vector3(-0.4162757f, -0.2012926f, -1.103625f),  // Box Collider Center
                new Vector3(3.456645f, 0.5974147f, 3.520729f)),  // Box Collider Size
            new ColliderDefinition("Collider_4",
                new Vector3(0.005f, 3.243f, 0.006f),    // Transform Position
                new Vector3(-0.0846022f, 0f, 0.0327953f),   // Box Collider Center
                new Vector3(1.803331f, 1f, 1.484335f))     // Box Collider Size
        ),

        // Placeable_SofaU01 - U-Sofa item
        new ItemColliderSet("Placeable_SofaU01",
            new ColliderDefinition("Collider_1",
                new Vector3(1.554f, 0.492f, -1.57f),    // Transform Position   
                new Vector3(0.0133402f, -0.1795357f, 0.369303f),    // Box Collider Center
                new Vector3(1.156934f, 0.5332844f, 2.366496f)),     // Box Collider Size

            new ColliderDefinition("Collider_2",
                new Vector3(-1.558f, 0.492f, -1.313f),    // Transform Position
                new Vector3(-0.0232008f, -0.1795357f, 0.539836f),   // Box Collider Center
                new Vector3(1.188833f, 0.5332844f, 1.331691f)),     // Box Collider Size

            new ColliderDefinition("Collider_3",
                new Vector3(0.524f, 0.492f, -0.103f),    // Transform Position
                new Vector3(-0.5338247f, -0.1795357f, -0.473882f),  // Box Collider Center
                new Vector3(4.313609f, 0.5332844f, 1.146815f)),     // Box Collider Size

            new ColliderDefinition("Collider_4",
                new Vector3(-1.672f, 0.901f, -0.325f),    // Transform Position
                new Vector3(-0.3996385f, -0.2961403f, -0.09090731f), // Box Collider Center
                new Vector3(0.2007231f, 0.407719f, 0.8181854f)),     // Box Collider Size

            new ColliderDefinition("Collider_5",
                new Vector3(0f, 0.69f, -0.507f),      // Transform Position
                new Vector3(-0.0042862f, 0.170211f, 0.3432779f),    // Box Collider Center
                new Vector3(3.892526f, 0.6595779f, 0.3360528f)),    // Box Collider Size

            new ColliderDefinition("Collider_6",
                new Vector3(0f, 0.69f, -1.549f),    // Transform Position
                new Vector3(1.968045f, 0.170211f, 0.6541931f), // Box Collider Center
                new Vector3(0.3543301f, 0.6595779f, 1.108435f)) // Box Collider Size
        ),

        // Placeable_TableDining01 - Dining Table
        new ItemColliderSet("Placeable_TableDining01",
            new ColliderDefinition("Collider_1",
                new Vector3(-0.466f, 0.323f, -0.177f),    // Transform Position
                new Vector3(-0.4001041f, 0.0498817f, -0.3738876f),    // Box Collider Center
                new Vector3(0.1578466f, 0.7191223f, 0.1436529f)),     // Box Collider Size

            new ColliderDefinition("Collider_2",
                new Vector3(1.256f, 0.323f, -0.179f),    // Transform Position
                new Vector3(-0.4001041f, 0.0498817f, -0.3738876f),    // Box Collider Center
                new Vector3(0.1578466f, 0.7191223f, 0.1436529f)),     // Box Collider Size

            new ColliderDefinition("Collider_3",
                new Vector3(1.256f, 0.323f, 0.927f),     // Transform Position
                new Vector3(-0.4001041f, 0.0498817f, -0.3738876f),    // Box Collider Center
                new Vector3(0.1578466f, 0.7191223f, 0.1436529f)),     // Box Collider Size

            new ColliderDefinition("Collider_4",
                new Vector3(-0.4541f, 0.323f, 0.925f),    // Transform Position
                new Vector3(-0.4001041f, 0.0498817f, -0.3738876f),    // Box Collider Center
                new Vector3(0.1578466f, 0.7191223f, 0.1436529f)),     // Box Collider Size

            new ColliderDefinition("Collider_5",
                new Vector3(-1.241f, 1.18f, 0.8f),        // Transform Position
                new Vector3(1.240966f, -0.429559f, -0.798788f),       // Box Collider Center
                new Vector3(2.467772f, 0.0492835f, 1.572596f))        // Box Collider Size
        ),

        // Placeable_TableCommon01 - Common Table
        new ItemColliderSet("Placeable_TableCommon01",
            new ColliderDefinition("Collider_1",
                new Vector3(0.225f, 0.395f, 0.024f),     // Transform Position
                new Vector3(0.4965301f, 0.00817276f, -0.0143563f),   // Box Collider Center
                new Vector3(0.1271404f, 0.7884314f, 0.548154f)),     // Box Collider Size

            new ColliderDefinition("Collider_2",
                new Vector3(-1.218f, 0.395f, 0.015f),    // Transform Position
                new Vector3(0.4965301f, 0.00817276f, -0.0143563f),   // Box Collider Center
                new Vector3(0.1271404f, 0.7884314f, 0.548154f)),     // Box Collider Size

            new ColliderDefinition("Collider_3",
                new Vector3(0.069f, 1.281f, 0f),         // Transform Position
                new Vector3(-0.0609390f, -0.4468388f, 0.00262277f),  // Box Collider Center
                new Vector3(1.910703f, 0.0971004f, 0.5040563f))      // Box Collider Size
        ),

        // Placeable_BedDouble - Double Bed
        new ItemColliderSet("Placeable_BedDouble",
            
            new ColliderDefinition("Collider_1",
                new Vector3(0f, 0.666f, -0.093f),                   // Transform Position
                new Vector3(-0.0053867f, -0.0104729f, 0.01526270f), // Box Collider Center
                new Vector3(1.929993f, 1.251071f, 0.1560026f)),     // Box Collider Size

            new ColliderDefinition("Collider_2",
                new Vector3(0.294f, 0.533f, -1.373f),               // Transform Position
                new Vector3(-0.2810981f, -0.221336f, 0.1454445f),   // Box Collider Center
                new Vector3(2.004876f, 0.5645964f, 2.294612f)),     // Box Collider Size

            new ColliderDefinition("Collider_3",
                new Vector3(1.447f, 0.216f, 0.568f),                // Transform Position
                new Vector3(-1.489491f, 0.405976f, -0.8889256f),    // Capsule Collider Center
                0.2521812f, 1.670227f, CapsuleDirection.X)          // Radius, Height, Direction
        ),

        // Placeable_Easel1 - Easel
        new ItemColliderSet("Placeable_Easel1",
            new ColliderDefinition("Collider_1",
                new Vector3(0.09f, 0.82f, -0.206f),                 // Transform Position
                new Vector3(-15f, 0f, 0f),                          // Transform Rotation (Euler)
                new Vector3(-0.089243f, 0.0529486f, 0.4028496f),    // Box Collider Center
                new Vector3(0.5740103f, 1.980013f, 0.0899536f)),    // Box Collider Size

            new ColliderDefinition("Collider_2",
                new Vector3(0.065f, 0.401f, -0.585f),               // Transform Position
                new Vector3(24f, 0f, 0f),                           // Transform Rotation (Euler)
                new Vector3(-0.066149f, 0.3674419f, 0.2962646f),    // Box Collider Center
                new Vector3(0.061027f, 1.291194f, 0.076293f))       // Box Collider Size
        ),

        // Placeable_WhiteBoard - Whiteboard
        new ItemColliderSet("Placeable_WhiteBoard",
            new ColliderDefinition("Collider_1",
                new Vector3(0f, 0.034f, -0.246f),                   // Transform Position
                new Vector3(0.01136374f, -0.02938062f, 0.2739586f), // Box Collider Center
                new Vector3(4.570652f, 1.784835f, 0.02464753f)),    // Box Collider Size

            new ColliderDefinition("Collider_2",
                new Vector3(0.782f, -0.945f, -0.189f),              // Transform Position
                new Vector3(-0.7895126f, 0.06564137f, 0.331845f),   // Box Collider Center
                new Vector3(4.409921f, 0.0180158f, 0.1806189f)),    // Box Collider Size

            new ColliderDefinition("pen_red",
                new Vector3(1.092f, -0.608f, 0.239f),
                new Vector3(0.1253498f, -0.238393f, -0.1144467f),
                new Vector3(0.3604466f, 0.03552473f, 0.1378608f)),

            new ColliderDefinition("pen_black",
                new Vector3(0.0804f, -0.6059f, 0.2415f),
                new Vector3(0.1253498f, -0.238393f, -0.1144467f),
                new Vector3(0.3604466f, 0.03552473f, 0.1378608f)),

            new ColliderDefinition("pen_blue",
                new Vector3(-0.3636f, -0.6059f, 0.2313f),
                new Vector3(0.1253498f, -0.238393f, -0.1144467f),
                new Vector3(0.3604466f, 0.03552473f, 0.1378608f)),

            new ColliderDefinition("Duster",
                new Vector3(-1.6245f, -0.6f, 0.2269f),
                new Vector3(0.1399584f, -0.2247734f, -0.1087361f),
                new Vector3(0.3896638f, 0.06276387f, 0.1411469f))
        ),


        // Add more items here in the same format:
        // new ItemColliderSet("ItemUniqueName",
        //     new ColliderDefinition("Collider_1", position, center, size),
        //     new ColliderDefinition("Collider_2", position, center, size),
        //     ...
        // ),
    };

    private static readonly Dictionary<string, ItemColliderSet> ColliderSetsByName =
        BuildColliderSetDictionary();

    private static Dictionary<string, ItemColliderSet> BuildColliderSetDictionary()
    {
        var dict = new Dictionary<string, ItemColliderSet>();
        foreach (var set in ColliderSets)
        {
            dict[set.ItemUniqueName] = set;
        }
        return dict;
    }


    // Check if a collider set exists for the given item
    public static bool HasColliderSet(string itemUniqueName)
    {
        return !string.IsNullOrEmpty(itemUniqueName) && ColliderSetsByName.ContainsKey(itemUniqueName);
    }


    // Apply special colliders to an item if a definition exists
    public static void ApplySpecialColliders(GameObject targetObject, string itemUniqueName, Mesh mesh)
    {
        if (targetObject == null)
            return;

        if (ColliderSetsByName.TryGetValue(itemUniqueName, out ItemColliderSet colliderSet))
        {
            // Remove any existing AdvancedCollision components from the visual and their colliders
            AdvancedCollision[] advancedCollisions = targetObject.GetComponentsInChildren<AdvancedCollision>(true);
            foreach (var ac in advancedCollisions)
            {
                if (ac != null)
                {
                    Collider[] colliders = ac.GetComponents<Collider>();
                    foreach (var col in colliders)
                    {
                        Object.Destroy(col);
                    }
                    Object.Destroy(ac);
                }
            }

            // Calculate visual offset based on mesh bounds (bottom-center anchor)
            Vector3 visualOffset = Vector3.zero;
            if (mesh != null)
            {
                Bounds bounds = mesh.bounds;
                visualOffset = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z);
            }

            // Whiteboard collider definitions are authored in the same local space as the placed visual,
            // so applying the generic visual offset shifts them upward incorrectly.
            if (itemUniqueName == "Placeable_WhiteBoard")
            {
                visualOffset = Vector3.zero;
            }
            
            ApplyColliderSet(targetObject, colliderSet, visualOffset);
        }
    }


    // Apply a collider set to a target object
    private static void ApplyColliderSet(GameObject targetObject, ItemColliderSet colliderSet, Vector3 visualOffset)
    {
        // Cache block layer lookup to avoid repeated string comparisons
        int blockLayer = LayerMask.NameToLayer("Block");
        Quaternion rootRotation = GetColliderRootRotation(colliderSet.ItemUniqueName);

        foreach (var colliderDef in colliderSet.Colliders)
        {
            // Apply visual offset to collider position to match visual centering
            Vector3 adjustedPosition = rootRotation * (colliderDef.LocalPosition + visualOffset);
            Vector3 adjustedRotationEuler = (rootRotation * Quaternion.Euler(colliderDef.LocalEulerRotation)).eulerAngles;
            
            if (colliderDef.Type == ColliderType.Box)
            {
                CreateColliderChild(targetObject, colliderDef.Name, 
                    adjustedPosition, adjustedRotationEuler, colliderDef.ColliderCenter, colliderDef.ColliderSize, blockLayer);
            }
            else if (colliderDef.Type == ColliderType.Capsule)
            {
                CreateCapsuleColliderChild(targetObject, colliderDef.Name, 
                    adjustedPosition, adjustedRotationEuler, colliderDef.ColliderCenter, colliderDef.CapsuleRadius, 
                    colliderDef.CapsuleHeight, colliderDef.Direction, blockLayer);
            }
        }

        // Apply layer settings
        if (blockLayer >= 0)
        {
            targetObject.layer = blockLayer;
        }
    }

    private static Quaternion GetColliderRootRotation(string itemUniqueName)
    {
        if (itemUniqueName == "Placeable_Easel1")
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }

        if (itemUniqueName == "Placeable_WhiteBoard")
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }

        return Quaternion.identity;
    }

    // Create a child GameObject with a box or capsule collider
    private static void CreateColliderChild(GameObject parent, string name, 
        Vector3 localPosition, Vector3 localRotationEuler, Vector3 colliderCenter, Vector3 colliderSize, int blockLayer)
    {
        GameObject colliderObj = new GameObject(name);
        colliderObj.transform.SetParent(parent.transform, false);
        colliderObj.transform.localPosition = localPosition;
        colliderObj.transform.localRotation = Quaternion.Euler(localRotationEuler);
        colliderObj.transform.localScale = Vector3.one;

        // Set child collider to Block layer
        if (blockLayer >= 0)
        {
            colliderObj.layer = blockLayer;
        }

        BoxCollider boxCollider = colliderObj.AddComponent<BoxCollider>();
        boxCollider.center = colliderCenter;
        boxCollider.size = colliderSize;
        boxCollider.isTrigger = false;

        colliderObj.AddComponent<AdvancedCollision>();
        
        colliderObj.AddComponent<SpecialItemCollider>();
    }

    // Create a child GameObject with a capsule collider
    private static void CreateCapsuleColliderChild(GameObject parent, string name, 
        Vector3 localPosition, Vector3 localRotationEuler, Vector3 colliderCenter, float radius, float height, CapsuleDirection direction, int blockLayer)
    {
        GameObject colliderObj = new GameObject(name);
        colliderObj.transform.SetParent(parent.transform, false);
        colliderObj.transform.localPosition = localPosition;
        colliderObj.transform.localRotation = Quaternion.Euler(localRotationEuler);
        colliderObj.transform.localScale = Vector3.one;

        // Set child collider to Block layer
        if (blockLayer >= 0)
        {
            colliderObj.layer = blockLayer;
        }

        CapsuleCollider capsuleCollider = colliderObj.AddComponent<CapsuleCollider>();
        capsuleCollider.center = colliderCenter;
        capsuleCollider.radius = radius;
        capsuleCollider.height = height;
        capsuleCollider.direction = (int)direction;
        capsuleCollider.isTrigger = false;

        // Add AdvancedCollision so game recognizes the collider
        colliderObj.AddComponent<AdvancedCollision>();
        
        // Add marker component so collision gate knows to manage this collider
        colliderObj.AddComponent<SpecialItemCollider>();
    }
}
