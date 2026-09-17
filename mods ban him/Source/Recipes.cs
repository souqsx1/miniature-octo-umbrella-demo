using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class test_balloons_recipes
{
    public static readonly CraftingCategory DecorCategory = (CraftingCategory)71;

    private static int nextCategoryOrder = 10000;

    public static ItemInstance_Recipe CreateRecipe(Item_Base sourceItem, string uniqueName)
    {
        if (sourceItem == null || sourceItem.settings_recipe == null)
        {
            return null;
        }

        ItemInstance_Recipe sourceRecipe = sourceItem.settings_recipe;
        ItemInstance_Recipe recipe = new ItemInstance_Recipe(
            DecorCategory,
            false,
            true,
            null,
            0);

        recipe.categoryOrder = nextCategoryOrder++;

        List<CostMultiple> costs = BuildCosts(uniqueName);
        if (costs.Count == 0)
        {
            Item_Base plasticFallback = ItemManager.GetItemByName("Plastic");
            if (plasticFallback != null)
            {
                costs.Add(new CostMultiple(new[] { plasticFallback }, 4));
            }
            else
            {
                costs.AddRange(GetSourceFallbackCosts(sourceRecipe));
            }
        }

        recipe.NewCost = costs.ToArray();
        return recipe;
    }

    private static List<CostMultiple> BuildCosts(string uniqueName)
    {
        List<CostMultiple> costs = new List<CostMultiple>();
        string key = uniqueName ?? string.Empty;

        switch (key)
        {
            case "Placeable_WallBalloons":
                AddCost(costs, "Plastic", 6);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_WallMirror":
                AddCost(costs, "Plank", 3);
                AddCost(costs, "Glass", 4);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_WallShower":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 1);
                AddCost(costs, "Hinge", 1);
                break;

            case "Placeable_OldCake":
                AddCost(costs, "Plank", 2);
                break;

            case "Placeable_Sofa01":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Nail", 4);
                AddCost(costs, "Wool", 1);
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_ComputerChassis":
            case "RT_CommRadio":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 1);
                AddCost(costs, "CircuitBoard", 1);
                break;

            case "Placeable_SoySack2":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Rope", 4);
                AddCost(costs, "Plastic", 1);
                AddCost(costs, "Palm Leaf", 15);
                break;

            case "Placeable_Book05":
                AddCost(costs, "Palm Leaf", 3);
                break;

            case "Placeable_Rug02":
                AddCost(costs, "Palm Leaf", 6);
                AddCost(costs, "Rope", 2);
                break;

            case "Placeable_WhiteBoard":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Plastic", 4);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Tablelamp01":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Bolt", 1);
                AddCost(costs, "CircuitBoard", 1);
                break;

            case "Placeable_Cube":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Bolt", 1);
                AddCost(costs, "CircuitBoard", 1);
                break;

            case "Placeable_Pooltable":
                AddCost(costs, "Plank", 14);
                AddCost(costs, "Nail", 8);
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_WallLamps":
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "CircuitBoard", 1);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_VentilationBox":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 2);
                AddCost(costs, "Hinge", 1);
                break;

            case "Placeable_Vent":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 2);
                AddCost(costs, "Hinge", 1);
                break;

            case "Placeable_BedsideTable":
                AddCost(costs, "Plank", 6);
                AddCost(costs, "Nail", 4);
                AddCost(costs, "Scrap", 1);
                break;

            case "Placeable_TowelHanger":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_HedgeBush1":
            case "Placeable_HedgeBush2":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Palm Leaf", 20);
                break;

            case "Placeable_Bowl02":
            case "Placeable_Bowl01":
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_PotBush1":
            case "Placeable_PotBush2":
                AddCost(costs, "Palm Leaf", 10);
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Clock02":
                AddCost(costs, "Plastic", 4);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_FlatScreen":
                AddCost(costs, "Plastic", 8);
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_Drawer01":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Nail", 5);
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_Flowerpot03":
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Plank", 1);
                break;

            case "Placeable_WBottle02":
                AddCost(costs, "Glass", 1);
                break;

            case "Placeable_Mirror01":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Glass", 3);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_OfficeChair":
                AddCost(costs, "Plank", 5);
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 2);
                break;

            case "Placeable_Armchair01":
                AddCost(costs, "Plank", 6);
                AddCost(costs, "Nail", 4);
                AddCost(costs, "Wool", 1);
                break;

            case "Placeable_PaintBrushJar":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Plank", 1);
                AddCost(costs, "Feather", 2);
                break;

            case "Placeable_PaintJarWhite":
            case "Placeable_PaintJarRed":
            case "Placeable_PaintJarBlue":
            case "Placeable_PaintJarBlack":
            case "Placeable_PaintJarGreen":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Scrap", 1);
                break;

            case "Placeable_CoffeeCup":
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_Shelf03":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 2);
                break;

            case "Placeable_Toiletpaper01":
                AddCost(costs, "Palm Leaf", 1);
                AddCost(costs, "Plastic", 1);
                break;

            case "Placeable_Vases02":
            case "Placeable_Vases03":
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_RolledTowels":
                AddCost(costs, "Wool", 1);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Walldecor02":
                AddCost(costs, "Plank", 3);
                AddCost(costs, "Nail", 2);
                AddCost(costs, "Plastic", 1);
                AddCost(costs, "Glass", 2);
                break;

            case "Placeable_SignWallLoadingBayUp":
            case "Placeable_TangaroaSign4":
            case "Placeable_TangaroaNumberSign6":
            case "Placeable_SignDoorSurfaceAccess":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_TangaroaPaperPile1":
                AddCost(costs, "Palm Leaf", 1);
                break;

            case "Placeable_TableDining01":
                AddCost(costs, "Plank", 12);
                AddCost(costs, "Nail", 8);
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_CoffeeMachine":
                AddCost(costs, "Plastic", 6);
                AddCost(costs, "Scrap", 5);
                AddCost(costs, "Bolt", 1);
                AddCost(costs, "CircuitBoard", 1);
                break;

            case "Placeable_Bottles01":
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_SunshadeWall":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Plastic", 4);
                AddCost(costs, "Palm Leaf", 14);
                AddCost(costs, "Rope", 2);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_SofaU01":
                AddCost(costs, "Plank", 15);
                AddCost(costs, "Nail", 6);
                AddCost(costs, "Wool", 1);
                AddCost(costs, "Plastic", 5);
                break;

            case "Placeable_Lamppost02":
                AddCost(costs, "CircuitBoard", 1);
                AddCost(costs, "Scrap", 10);
                break;

            case "Placeable_Tray":
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_TrayHolder1":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Ashtray01":
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_Rug01":
                AddCost(costs, "Palm Leaf", 6);
                AddCost(costs, "Rope", 2);
                break;

            case "Placeable_TangaroaPaperPile2":
                AddCost(costs, "Palm Leaf", 2);
                break;

            case "Placeable_TangaroaPaperPile3":
                AddCost(costs, "Palm Leaf", 3);
                break;

            case "Placeable_TangaroaNumberSign3":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_SignDoorStorageArea":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_Easel1":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Walldecor03":
                AddCost(costs, "Plank", 3);
                AddCost(costs, "Nail", 2);
                AddCost(costs, "Plastic", 1);
                AddCost(costs, "Glass", 2);
                break;

            case "Placeable_PottedSoyPlant2":
                AddCost(costs, "Palm Leaf", 8);
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Dinnerchair01":
                AddCost(costs, "Plank", 5);
                AddCost(costs, "Nail", 4);
                break;

            case "Placeable_TangaroaPlantsSmallPlant1":
                AddCost(costs, "Palm Leaf", 5);
                AddCost(costs, "Plank", 1);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_TangaroaPlantsSmallPlant2":
                AddCost(costs, "Palm Leaf", 6);
                AddCost(costs, "Plank", 1);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Container02":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Nail", 3);
                AddCost(costs, "Hinge", 1);
                break;

            case "SatelliteDish":
            case "RT_WallVent":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Nail", 3);
                AddCost(costs, "Hinge", 1);
                break;

            case "Placeable_StairwellWallLamp":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "CircuitBoard", 1);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Tub01":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Hinge", 1);
                break;

            case "Placeable_TangaroaKitchenBench5Sink":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 2);
                break;

            case "Placeable_Firehydrant01":
                AddCost(costs, "Scrap", 8);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_TangaroaHoseWheel":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_Carpet02":
                AddCost(costs, "Palm Leaf", 4);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Book01":
            case "Placeable_Book02":
            case "Placeable_Book03":
            case "Placeable_Book04":
            case "Placeable_Book06":
            case "Placeable_Book07":
            case "Placeable_Book08":
                AddCost(costs, "Palm Leaf", 6);
                break;

            case "Placeable_TangaroaKitchenFridge":
                AddCost(costs, "Plank", 12);
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 2);
                AddCost(costs, "CircuitBoard", 1);
                break;

            case "RTPowerBox":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 2);
                AddCost(costs, "CircuitBoard", 1);
                break;

            case "Placeable_TangaroaKitchenIslandWineShelf":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Nail", 6);
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_Mouse":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Scrap", 1);
                break;

            case "Placeable_Keyboard":
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_Carpet03":
                AddCost(costs, "Palm Leaf", 4);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_TangaroaNumberSign4":
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_OutdoorBench":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Nail", 4);
                break;

            case "Placeable_Shelf01":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 2);
                break;

            case "Placeable_Shelf02":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 2);
                break;

            case "Placeable_Shelf":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 2);
                break;

            case "Placeable_PresentOpen":
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_TangaroaKitchenCuttingBoard":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Scrap", 1);
                break;

            case "Placeable_Rug03":
                AddCost(costs, "Palm Leaf", 6);
                AddCost(costs, "Rope", 2);
                break;

            case "Placeable_SoySackPile2":
            case "Placeable_SoySackPile1":
            case "Placeable_SoySackPile3":
                AddCost(costs, "Rope", 2);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Palm Leaf", 5);
                break;

            case "Placeable_TangaroaSign5":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_ChandelierOn":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "CircuitBoard", 1);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_Rug04":
                AddCost(costs, "Palm Leaf", 6);
                AddCost(costs, "Rope", 2);
                break;

            case "Placeable_TangaroaPlantsSmallPlant3":
                AddCost(costs, "Palm Leaf", 6);
                AddCost(costs, "Plank", 1);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_TableCommon01":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Nail", 4);
                break;

            case "Placeable_Toilet01":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Bolt", 2);
                AddCost(costs, "Hinge", 1);
                break;

            case "Placeable_TangaroaNumberSign1":
            case "Placeable_TangaroaNumberSign2":
            case "Placeable_TangaroaNumberSign8":
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_PartyHat":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Palm Leaf", 1);
                break;

            case "Placeable_SunshadeGround":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Plastic", 4);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Clock01":
                AddCost(costs, "Plastic", 4);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_Vases01":
                AddCost(costs, "Clay", 2);
                break;

            case "Placeable_Lamppost01":
                AddCost(costs, "Glass", 1);
                AddCost(costs, "Scrap", 4);
                break;

            case "Placeable_RestaFence":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Nail", 3);
                break;

            case "Placeable_TangaroaBucket":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_PresentPile2":
            case "RT_Notepad":
                AddCost(costs, "Rope", 3);
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_TangaroaWheelBarrow":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Scrap", 5);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_OutdoorTable":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Nail", 6);
                break;

            case "Placeable_Utensolholder01":
                AddCost(costs, "Plank", 3);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Soap01":
                AddCost(costs, "Plastic", 1);
                break;

            case "Placeable_Bedsidetable01":
                AddCost(costs, "Plank", 6);
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_WallBalloons1":
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_TangaroaBench":
                AddCost(costs, "Plank", 8);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 3);
                break;

            case "Placeable_SignDoorCafeteria":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Plastic", 3);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_ChartBoard":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_TangaroaSign3":
                AddCost(costs, "Plank", 3);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_TangaroaSign2":
                AddCost(costs, "Plank", 3);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_Carpet01":
                AddCost(costs, "Palm Leaf", 5);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Closet01":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Nail", 6);
                AddCost(costs, "Scrap", 4);
                break;

            case "Placeable_SignWallSurfaceAccessUp":
            case "Placeable_TangaroaSign1":
            case "Placeable_SignWallPlantation":
            case "Placeable_SignDoorPlantation":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Scrap", 1);
                AddCost(costs, "Nail", 2);
                break;

            case "Placeable_Floorlamp01":
                AddCost(costs, "Scrap", 5);
                AddCost(costs, "Bolt", 2);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_Plate01":
                AddCost(costs, "Clay", 2);
                break;

            case "Placeable_CanvasStackBlank":
                AddCost(costs, "Plank", 4);
                AddCost(costs, "Palm Leaf", 3);
                AddCost(costs, "Plastic", 1);
                break;

            case "Placeable_CanvasBlank":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Palm Leaf", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_Flowerpot01":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Clay", 2);
                break;

            case "Placeable_TangaroaNumberSign5":
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_Trashcan01":
                AddCost(costs, "Scrap", 6);
                AddCost(costs, "Nail", 2);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_OutOfOrderPoster":
                AddCost(costs, "Palm Leaf", 2);
                break;

            case "Placeable_Painting1":
            case "Placeable_Painting2":
            case "Placeable_Painting3":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Plastic", 1);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_Hanger01":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Nail", 2);
                AddCost(costs, "Scrap", 1);
                break;

            case "Placeable_Mousepad":
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Palm Leaf", 1);
                break;

            case "Placeable_PresentPile1":
                AddCost(costs, "Rope", 2);
                AddCost(costs, "Plastic", 2);
                break;

            case "Placeable_TangaroaBathroomSink":
                AddCost(costs, "Plank", 6);
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_Metalpipe02":
            case "Placeable_Metalpipe01":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_BedDouble":
                AddCost(costs, "Plank", 16);
                AddCost(costs, "Nail", 8);
                AddCost(costs, "Wool", 2);
                break;

            case "Placeable_BedSmall":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Nail", 5);
                AddCost(costs, "Palm Leaf", 20);
                break;

            case "Placeable_Bookstop01":
                AddCost(costs, "Scrap", 3);
                AddCost(costs, "Plastic", 1);
                break;

            case "Placeable_Cabinet01":
                AddCost(costs, "Plank", 10);
                AddCost(costs, "Nail", 6);
                AddCost(costs, "Scrap", 2);
                break;

            case "Placeable_PottedSoyPlant1":
                AddCost(costs, "Palm Leaf", 8);
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Rope", 1);
                break;

            case "Placeable_RtClipboard":
                AddCost(costs, "Plank", 2);
                AddCost(costs, "Plastic", 1);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_FireEstinguisher":
                AddCost(costs, "Scrap", 4);
                AddCost(costs, "Plastic", 2);
                AddCost(costs, "Bolt", 1);
                break;

            case "Placeable_BoxSmall":
                AddCost(costs, "Plank", 4);
                break;

            case "Placeable_SharedAssets6Table":
                AddCost(costs, "Plank", 6);
                AddCost(costs, "Scrap", 2);
                AddCost(costs, "Nail", 1);
                break;

            case "Placeable_RtCameraBase":
                AddCost(costs, "Scrap", 5);
                AddCost(costs, "Bolt", 1);
                AddCost(costs, "CircuitBoard", 1);
                break;
        }

        // Fallback for any future item IDs added later.
        if (costs.Count == 0)
        {
            AddCost(costs, "Plank", 8);
            AddCost(costs, "Plastic", 4);
            AddCost(costs, "Scrap", 3);
            AddCost(costs, "Nail", 3);
        }

        return costs;
    }

    private static void AddCost(List<CostMultiple> costs, string itemName, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Item_Base item = ResolveItem(itemName);
        if (item == null)
        {
            return;
        }

        costs.Add(new CostMultiple(new[] { item }, amount));
    }

    private static Item_Base ResolveItem(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
        {
            return null;
        }

        Item_Base item = ItemManager.GetItemByName(itemName);
        if (item != null)
        {
            return item;
        }

        // Handle common naming variants across game/mod loader versions.
        if (itemName.Contains(" "))
        {
            item = ItemManager.GetItemByName(itemName.Replace(" ", string.Empty));
            if (item != null)
            {
                return item;
            }

            item = ItemManager.GetItemByName(itemName.Replace(" ", "_"));
            if (item != null)
            {
                return item;
            }
        }

        if (string.Equals(itemName, "Palm Leaf", StringComparison.OrdinalIgnoreCase))
        {
            item = ItemManager.GetItemByName("PalmLeaf") ?? ItemManager.GetItemByName("Palm_Leaf");
            if (item != null)
            {
                return item;
            }

            return TryResolveByIndex(25);
        }

        return null;
    }

    private static Item_Base TryResolveByIndex(int itemIndex)
    {
        try
        {
            MethodInfo getByIndex = typeof(ItemManager).GetMethod("GetItemByIndex", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(int) }, null);
            if (getByIndex == null)
            {
                return null;
            }

            return getByIndex.Invoke(null, new object[] { itemIndex }) as Item_Base;
        }
        catch
        {
            return null;
        }
    }

    private static IEnumerable<CostMultiple> GetSourceFallbackCosts(ItemInstance_Recipe sourceRecipe)
    {
        if (sourceRecipe == null || sourceRecipe.NewCost == null)
        {
            return Array.Empty<CostMultiple>();
        }

        return sourceRecipe.NewCost.Where(cost => cost != null);
    }
}
