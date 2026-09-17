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
    private const string BalloonMeshName = "WallBalloons3";
    private const string CustomItemUniqueName = "Placeable_WallBalloons";
    private const int CustomItemUniqueIndex = 10000;
    private const string MirrorMeshName = "Tangaroa_Bathroom_Mirror";
    private const string MirrorItemUniqueName = "Placeable_WallMirror";
    private const int MirrorItemUniqueIndex = 10001;
    private const string ShowerMeshName = "Shower_01";
    private const string ShowerItemUniqueName = "Placeable_WallShower";
    private const int ShowerItemUniqueIndex = 10002;
    private const string CakeMeshName = "OldCake";
    private const string CakeItemUniqueName = "Placeable_OldCake";
    private const int CakeItemUniqueIndex = 10003;
    private const string SofaMeshName = "Sofa_01";
    private const string SofaItemUniqueName = "Placeable_Sofa01";
    private const int SofaItemUniqueIndex = 10004;
    private const string ComputerChassisMeshName = "ComputerChassis";
    private const string ComputerChassisItemUniqueName = "Placeable_ComputerChassis";
    private const int ComputerChassisItemUniqueIndex = 10005;
    private const string SoySackMeshName = "SoySack2";
    private const string SoySackItemUniqueName = "Placeable_SoySack2";
    private const int SoySackItemUniqueIndex = 10006;
    private const string BookMeshName = "Book_05";
    private const string BookItemUniqueName = "Placeable_Book05";
    private const int BookItemUniqueIndex = 10007;
    private const string RugMeshName = "Rug_02";
    private const string RugItemUniqueName = "Placeable_Rug02";
    private const int RugItemUniqueIndex = 10008;
    private const string WhiteBoardMeshName = "WhiteBoard";
    private const string WhiteBoardItemUniqueName = "Placeable_WhiteBoard";
    private const int WhiteBoardItemUniqueIndex = 10009;
    private const string TablelampMeshName = "Tablelamp_01";
    private const string TablelampItemUniqueName = "Placeable_Tablelamp01";
    private const int TablelampItemUniqueIndex = 10010;
    private const string PoolTableMeshName = "Pooltable";
    private const string PoolTableItemUniqueName = "Placeable_Pooltable";
    private const int PoolTableItemUniqueIndex = 10011;
    private const string WallLampsMeshName = "WallLamps";
    private const string WallLampsItemUniqueName = "Placeable_WallLamps";
    private const int WallLampsItemUniqueIndex = 10012;
    private const string VentilationBoxMeshName = "Ventilation_Box";
    private const string VentilationBoxItemUniqueName = "Placeable_VentilationBox";
    private const int VentilationBoxItemUniqueIndex = 10013;
    private const string BedsideTableMeshName = "Bedside_Table";
    private const string BedsideTableItemUniqueName = "Placeable_BedsideTable";
    private const int BedsideTableItemUniqueIndex = 10014;
    private const string TowelHangerMeshName = "TowelHanger";
    private const string TowelHangerItemUniqueName = "Placeable_TowelHanger";
    private const int TowelHangerItemUniqueIndex = 10015;
    private const string HedgeBush1MeshName = "HedgeBush1";
    private const string HedgeBush1ItemUniqueName = "Placeable_HedgeBush1";
    private const int HedgeBush1ItemUniqueIndex = 10016;
    private const string HedgeBush2MeshName = "HedgeBush2";
    private const string HedgeBush2ItemUniqueName = "Placeable_HedgeBush2";
    private const int HedgeBush2ItemUniqueIndex = 10017;
    private const string Bowl02MeshName = "Bowl_02";
    private const string Bowl02ItemUniqueName = "Placeable_Bowl02";
    private const int Bowl02ItemUniqueIndex = 10018;
    private const string PotBush1MeshName = "PotBush1";
    private const string PotBush1ItemUniqueName = "Placeable_PotBush1";
    private const int PotBush1ItemUniqueIndex = 10019;
    private const string PotBush2MeshName = "PotBush2";
    private const string PotBush2ItemUniqueName = "Placeable_PotBush2";
    private const int PotBush2ItemUniqueIndex = 10020;
    private const string Clock02MeshName = "Clock_02";
    private const string Clock02ItemUniqueName = "Placeable_Clock02";
    private const int Clock02ItemUniqueIndex = 10021;
    private const string FlatScreenMeshName = "FlatScreen";
    private const string FlatScreenItemUniqueName = "Placeable_FlatScreen";
    private const int FlatScreenItemUniqueIndex = 10022;
    private const string Drawer01MeshName = "Drawer_01";
    private const string Drawer01ItemUniqueName = "Placeable_Drawer01";
    private const int Drawer01ItemUniqueIndex = 10023;
    private const string Flowerpot03MeshName = "Flowerpot_03";
    private const string Flowerpot03ItemUniqueName = "Placeable_Flowerpot03";
    private const int Flowerpot03ItemUniqueIndex = 10024;
    private const string WBottle02MeshName = "WBottle_02";
    private const string WBottle02ItemUniqueName = "Placeable_WBottle02";
    private const int WBottle02ItemUniqueIndex = 10025;
    private const string OfficeChairMeshName = "OfficeChair";
    private const string OfficeChairItemUniqueName = "Placeable_OfficeChair";
    private const int OfficeChairItemUniqueIndex = 10026;
    private const string PaintBrushJarMeshName = "PaintBrushJar";
    private const string PaintBrushJarItemUniqueName = "Placeable_PaintBrushJar";
    private const int PaintBrushJarItemUniqueIndex = 10027;
    private const string PaintJarWhiteMeshName = "PaintJar_White";
    private const string PaintJarWhiteItemUniqueName = "Placeable_PaintJarWhite";
    private const int PaintJarWhiteItemUniqueIndex = 10028;
    private const string PaintJarRedMeshName = "PaintJar_Red";
    private const string PaintJarRedItemUniqueName = "Placeable_PaintJarRed";
    private const int PaintJarRedItemUniqueIndex = 10029;
    private const string PaintJarBlueMeshName = "PaintJar_Blue";
    private const string PaintJarBlueItemUniqueName = "Placeable_PaintJarBlue";
    private const int PaintJarBlueItemUniqueIndex = 10030;
    private const string PaintJarBlackMeshName = "PaintJar_Black";
    private const string PaintJarBlackItemUniqueName = "Placeable_PaintJarBlack";
    private const int PaintJarBlackItemUniqueIndex = 10031;
    private const string PaintJarGreenMeshName = "PaintJar_Green";
    private const string PaintJarGreenItemUniqueName = "Placeable_PaintJarGreen";
    private const int PaintJarGreenItemUniqueIndex = 10032;
    private const string CoffeeCupMeshName = "CoffeeCup";
    private const string CoffeeCupItemUniqueName = "Placeable_CoffeeCup";
    private const int CoffeeCupItemUniqueIndex = 10033;
    private const string Shelf03MeshName = "Shelf_03";
    private const string Shelf03ItemUniqueName = "Placeable_Shelf03";
    private const int Shelf03ItemUniqueIndex = 10034;
    private const string Toiletpaper01MeshName = "Toiletpaper_01";
    private const string Toiletpaper01ItemUniqueName = "Placeable_Toiletpaper01";
    private const int Toiletpaper01ItemUniqueIndex = 10035;
    private const string Vases02MeshName = "Vases_02";
    private const string Vases02ItemUniqueName = "Placeable_Vases02";
    private const int Vases02ItemUniqueIndex = 10036;
    private const string Vases03MeshName = "Vases_03";
    private const string Vases03ItemUniqueName = "Placeable_Vases03";
    private const int Vases03ItemUniqueIndex = 10037;
    private const string RolledTowelsMeshName = "Tangaroa_Bathroom_RolledTowels";
    private const string RolledTowelsItemUniqueName = "Placeable_RolledTowels";
    private const int RolledTowelsItemUniqueIndex = 10038;
    private const string WallDecor02MeshName = "Walldecor_02";
    private const string WallDecor02ItemUniqueName = "Placeable_Walldecor02";
    private const int WallDecor02ItemUniqueIndex = 10039;
    private const string WallDecor03MeshName = "Walldecor_03";
    private const string WallDecor03ItemUniqueName = "Placeable_Walldecor03";
    private const int WallDecor03ItemUniqueIndex = 10049;
    private const string SignWallLoadingBayUpMeshName = "SignWall_LoadingBayUp";
    private const string SignWallLoadingBayUpItemUniqueName = "Placeable_SignWallLoadingBayUp";
    private const int SignWallLoadingBayUpItemUniqueIndex = 10040;
    private const string TangaroaPaperPile1MeshName = "TangaroaPaperPile1";
    private const string TangaroaPaperPile1ItemUniqueName = "Placeable_TangaroaPaperPile1";
    private const int TangaroaPaperPile1ItemUniqueIndex = 10041;
    private const string TangaroaSign4MeshName = "TangaroaSign4";
    private const string TangaroaSign4ItemUniqueName = "Placeable_TangaroaSign4";
    private const int TangaroaSign4ItemUniqueIndex = 10042;
    private const string TableDining01MeshName = "Table_Dining_01";
    private const string TableDining01ItemUniqueName = "Placeable_TableDining01";
    private const int TableDining01ItemUniqueIndex = 10043;
    private const string CoffeeMachineMeshName = "CoffeeMachine";
    private const string CoffeeMachineItemUniqueName = "Placeable_CoffeeMachine";
    private const int CoffeeMachineItemUniqueIndex = 10044;
    private const string TangaroaNumberSign6MeshName = "TangaroaNumberSign6";
    private const string TangaroaNumberSign6ItemUniqueName = "Placeable_TangaroaNumberSign6";
    private const int TangaroaNumberSign6ItemUniqueIndex = 10045;
    private const string Bottles01MeshName = "Bottles_01";
    private const string Bottles01ItemUniqueName = "Placeable_Bottles01";
    private const int Bottles01ItemUniqueIndex = 10046;
    private const string SignDoorSurfaceAccessMeshName = "SignDoor_SurfaceAccess";
    private const string SignDoorSurfaceAccessItemUniqueName = "Placeable_SignDoorSurfaceAccess";
    private const int SignDoorSurfaceAccessItemUniqueIndex = 10047;
    private const string PottedSoyPlant2MeshName = "PottedSoyPlant2";
    private const string PottedSoyPlant2ItemUniqueName = "Placeable_PottedSoyPlant2";
    private const int PottedSoyPlant2ItemUniqueIndex = 10050;
    private const string Dinnerchair01MeshName = "Dinnerchair_01";
    private const string Dinnerchair01ItemUniqueName = "Placeable_Dinnerchair01";
    private const int Dinnerchair01ItemUniqueIndex = 10051;
    private const string TangaroaPlantsSmallPlant2MeshName = "TangaroaPlants_SmallPlant2";
    private const string TangaroaPlantsSmallPlant2ItemUniqueName = "Placeable_TangaroaPlantsSmallPlant2";
    private const int TangaroaPlantsSmallPlant2ItemUniqueIndex = 10052;
    private const string Container02MeshName = "Container_02";
    private const string Container02ItemUniqueName = "Placeable_Container02";
    private const int Container02ItemUniqueIndex = 10053;
    private const string StairwellWallLampMeshName = "Stairwell_WallLamp";
    private const string StairwellWallLampItemUniqueName = "Placeable_StairwellWallLamp";
    private const int StairwellWallLampItemUniqueIndex = 10054;
    private const string Tub01MeshName = "Tub_01";
    private const string Tub01ItemUniqueName = "Placeable_Tub01";
    private const int Tub01ItemUniqueIndex = 10055;
    private const string TangaroaKitchenBench5SinkMeshName = "TangaroaKitchen_Bench5_Sink";
    private const string TangaroaKitchenBench5SinkItemUniqueName = "Placeable_TangaroaKitchenBench5Sink";
    private const int TangaroaKitchenBench5SinkItemUniqueIndex = 10056;
    private const string Firehydrant01MeshName = "Firehydrant_01";
    private const string Firehydrant01ItemUniqueName = "Placeable_Firehydrant01";
    private const int Firehydrant01ItemUniqueIndex = 10057;
    private const string TangaroaHoseWheelMeshName = "TangaroaHoseWheel";
    private const string TangaroaHoseWheelItemUniqueName = "Placeable_TangaroaHoseWheel";
    private const int TangaroaHoseWheelItemUniqueIndex = 10058;
    private const string Carpet02MeshName = "Carpet_02";
    private const string Carpet02ItemUniqueName = "Placeable_Carpet02";
    private const int Carpet02ItemUniqueIndex = 10059;
    private const string Book01MeshName = "Book_01";
    private const string Book01ItemUniqueName = "Placeable_Book01";
    private const int Book01ItemUniqueIndex = 10060;
    private const string Book02MeshName = "Book_02";
    private const string Book02ItemUniqueName = "Placeable_Book02";
    private const int Book02ItemUniqueIndex = 10061;
    private const string Book03MeshName = "Book_03";
    private const string Book03ItemUniqueName = "Placeable_Book03";
    private const int Book03ItemUniqueIndex = 10062;
    private const string Book04MeshName = "Book_04";
    private const string Book04ItemUniqueName = "Placeable_Book04";
    private const int Book04ItemUniqueIndex = 10063;
    private const string Book06MeshName = "Book_06";
    private const string Book06ItemUniqueName = "Placeable_Book06";
    private const int Book06ItemUniqueIndex = 10064;
    private const string Book07MeshName = "Book_07";
    private const string Book07ItemUniqueName = "Placeable_Book07";
    private const int Book07ItemUniqueIndex = 10065;
    private const string Book08MeshName = "Book_08";
    private const string Book08ItemUniqueName = "Placeable_Book08";
    private const int Book08ItemUniqueIndex = 10066;
    private const string TangaroaKitchenFridgeMeshName = "TangaroaKitchen_Fridge";
    private const string TangaroaKitchenFridgeItemUniqueName = "Placeable_TangaroaKitchenFridge";
    private const int TangaroaKitchenFridgeItemUniqueIndex = 10067;
    private const string TangaroaKitchenIslandWineShelfMeshName = "TangaroaKitchen_Island_WineShelf";
    private const string TangaroaKitchenIslandWineShelfItemUniqueName = "Placeable_TangaroaKitchenIslandWineShelf";
    private const int TangaroaKitchenIslandWineShelfItemUniqueIndex = 10068;
    private const string MouseMeshName = "Mouse";
    private const string MouseItemUniqueName = "Placeable_Mouse";
    private const int MouseItemUniqueIndex = 10069;
    private const string Carpet03MeshName = "Carpet_03";
    private const string Carpet03ItemUniqueName = "Placeable_Carpet03";
    private const int Carpet03ItemUniqueIndex = 10070;
    private const string TangaroaNumberSign4MeshName = "TangaroaNumberSign4";
    private const string TangaroaNumberSign4ItemUniqueName = "Placeable_TangaroaNumberSign4";
    private const int TangaroaNumberSign4ItemUniqueIndex = 10071;
    private const string OutdoorBenchMeshName = "OutdoorBench";
    private const string OutdoorBenchItemUniqueName = "Placeable_OutdoorBench";
    private const int OutdoorBenchItemUniqueIndex = 10072;
    private const string Shelf02MeshName = "Shelf_02";
    private const string Shelf02ItemUniqueName = "Placeable_Shelf02";
    private const int Shelf02ItemUniqueIndex = 10073;
    private const string PresentOpenMeshName = "PresentOpen";
    private const string PresentOpenItemUniqueName = "Placeable_PresentOpen";
    private const int PresentOpenItemUniqueIndex = 10074;
    private const string TangaroaKitchenCuttingBoardMeshName = "TangaroaKitchen_CuttingBoard";
    private const string TangaroaKitchenCuttingBoardItemUniqueName = "Placeable_TangaroaKitchenCuttingBoard";
    private const int TangaroaKitchenCuttingBoardItemUniqueIndex = 10075;
    private const string Rug03MeshName = "Rug_03";
    private const string Rug03ItemUniqueName = "Placeable_Rug03";
    private const int Rug03ItemUniqueIndex = 10076;
    private const string SoySackPile2MeshName = "SoySackPile2";
    private const string SoySackPile2ItemUniqueName = "Placeable_SoySackPile2";
    private const int SoySackPile2ItemUniqueIndex = 10077;
    private const string TangaroaSign5MeshName = "TangaroaSign5";
    private const string TangaroaSign5ItemUniqueName = "Placeable_TangaroaSign5";
    private const int TangaroaSign5ItemUniqueIndex = 10078;
    private const string KeyboardMeshName = "Keyboard";
    private const string KeyboardItemUniqueName = "Placeable_Keyboard";
    private const int KeyboardItemUniqueIndex = 10115;
    private const string Rug04MeshName = "Rug_04";
    private const string Rug04ItemUniqueName = "Placeable_Rug04";
    private const int Rug04ItemUniqueIndex = 10079;
    private const string TangaroaPlantsSmallPlant3MeshName = "TangaroaPlants_SmallPlant3";
    private const string TangaroaPlantsSmallPlant3ItemUniqueName = "Placeable_TangaroaPlantsSmallPlant3";
    private const int TangaroaPlantsSmallPlant3ItemUniqueIndex = 10080;
    private const string TableCommon01MeshName = "Table_Common_01";
    private const string TableCommon01ItemUniqueName = "Placeable_TableCommon01";
    private const int TableCommon01ItemUniqueIndex = 10081;
    private const string Toilet01MeshName = "Toilet_01";
    private const string Toilet01ItemUniqueName = "Placeable_Toilet01";
    private const int Toilet01ItemUniqueIndex = 10082;
    private const string TangaroaNumberSign1MeshName = "TangaroaNumberSign1";
    private const string TangaroaNumberSign1ItemUniqueName = "Placeable_TangaroaNumberSign1";
    private const int TangaroaNumberSign1ItemUniqueIndex = 10084;
    private const string PartyHatMeshName = "PartyHat";
    private const string PartyHatItemUniqueName = "Placeable_PartyHat";
    private const int PartyHatItemUniqueIndex = 10086;
    private const string SunshadeGroundMeshName = "Sunshade_Ground";
    private const string SunshadeGroundItemUniqueName = "Placeable_SunshadeGround";
    private const int SunshadeGroundItemUniqueIndex = 10087;
    private const string Clock01MeshName = "Clock_01";
    private const string Clock01ItemUniqueName = "Placeable_Clock01";
    private const int Clock01ItemUniqueIndex = 10088;
    private const string Vases01MeshName = "Vases_01";
    private const string Vases01ItemUniqueName = "Placeable_Vases01";
    private const int Vases01ItemUniqueIndex = 10089;
    private const string Lamppost01MeshName = "Lamppost_01";
    private const string Lamppost01ItemUniqueName = "Placeable_Lamppost01";
    private const int Lamppost01ItemUniqueIndex = 10090;
    private const string RestaFenceMeshName = "RestaFence";
    private const string RestaFenceItemUniqueName = "Placeable_RestaFence";
    private const int RestaFenceItemUniqueIndex = 10091;
    private const string TangaroaBucketMeshName = "TangaroaBucket";
    private const string TangaroaBucketItemUniqueName = "Placeable_TangaroaBucket";
    private const int TangaroaBucketItemUniqueIndex = 10092;
    private const string PresentPile2MeshName = "PresentPile2";
    private const string PresentPile2ItemUniqueName = "Placeable_PresentPile2";
    private const int PresentPile2ItemUniqueIndex = 10093;
    private const string TangaroaWheelBarrowMeshName = "TangaroaWheelBarrow";
    private const string TangaroaWheelBarrowItemUniqueName = "Placeable_TangaroaWheelBarrow";
    private const int TangaroaWheelBarrowItemUniqueIndex = 10095;
    private const string OutdoorTableMeshName = "OutdoorTable";
    private const string OutdoorTableItemUniqueName = "Placeable_OutdoorTable";
    private const int OutdoorTableItemUniqueIndex = 10096;
    private const string Utensolholder01MeshName = "Utensolholder_01";
    private const string Utensolholder01ItemUniqueName = "Placeable_Utensolholder01";
    private const int Utensolholder01ItemUniqueIndex = 10097;
    private const string Soap01MeshName = "Soap_01";
    private const string Soap01ItemUniqueName = "Placeable_Soap01";
    private const int Soap01ItemUniqueIndex = 10098;
    private const string SunshadeWallMeshName = "Sunshade_Wall";
    private const string SunshadeWallItemUniqueName = "Placeable_SunshadeWall";
    private const int SunshadeWallItemUniqueIndex = 10099;
    private const string SofaU01MeshName = "SofaU_01";
    private const string SofaU01ItemUniqueName = "Placeable_SofaU01";
    private const int SofaU01ItemUniqueIndex = 10100;
    private const string Lamppost02MeshName = "Lamppost_02";
    private const string Lamppost02ItemUniqueName = "Placeable_Lamppost02";
    private const int Lamppost02ItemUniqueIndex = 10101;
    private const string TrayMeshName = "Tray";
    private const string TrayItemUniqueName = "Placeable_Tray";
    private const int TrayItemUniqueIndex = 10102;
    private const string Rug01MeshName = "Rug_01";
    private const string Rug01ItemUniqueName = "Placeable_Rug01";
    private const int Rug01ItemUniqueIndex = 10103;
    private const string TangaroaPaperPile3MeshName = "TangaroaPaperPile3";
    private const string TangaroaPaperPile3ItemUniqueName = "Placeable_TangaroaPaperPile3";
    private const int TangaroaPaperPile3ItemUniqueIndex = 10104;
    private const string TangaroaNumberSign3MeshName = "TangaroaNumberSign3";
    private const string TangaroaNumberSign3ItemUniqueName = "Placeable_TangaroaNumberSign3";
    private const int TangaroaNumberSign3ItemUniqueIndex = 10105;
    private const string SignDoorStorageAreaMeshName = "SignDoor_StorageArea";
    private const string SignDoorStorageAreaItemUniqueName = "Placeable_SignDoorStorageArea";
    private const int SignDoorStorageAreaItemUniqueIndex = 10106;
    private const string Easel1MeshName = "Easel1";
    private const string Easel1ItemUniqueName = "Placeable_Easel1";
    private const int Easel1ItemUniqueIndex = 10107;
    private const string Bedsidetable01MeshName = "Bedsidetable_01";
    private const string Bedsidetable01ItemUniqueName = "Placeable_Bedsidetable01";
    private const int Bedsidetable01ItemUniqueIndex = 10108;
    private const string WallBalloons1MeshName = "WallBalloons1";
    private const string WallBalloons1ItemUniqueName = "Placeable_WallBalloons1";
    private const int WallBalloons1ItemUniqueIndex = 10109;
    private const string TangaroaBenchMeshName = "TangaroaBench";
    private const string TangaroaBenchItemUniqueName = "Placeable_TangaroaBench";
    private const int TangaroaBenchItemUniqueIndex = 10110;
    private const string SignDoorCafeteriaMeshName = "SignDoor_Cafeteria";
    private const string SignDoorCafeteriaItemUniqueName = "Placeable_SignDoorCafeteria";
    private const int SignDoorCafeteriaItemUniqueIndex = 10111;
    private const string ChartBoardMeshName = "ChartBoard";
    private const string ChartBoardItemUniqueName = "Placeable_ChartBoard";
    private const int ChartBoardItemUniqueIndex = 10112;
    private const string TangaroaSign3MeshName = "TangaroaSign3";
    private const string TangaroaSign3ItemUniqueName = "Placeable_TangaroaSign3";
    private const int TangaroaSign3ItemUniqueIndex = 10113;
    private const string Carpet01MeshName = "Carpet_01";
    private const string Carpet01ItemUniqueName = "Placeable_Carpet01";
    private const int Carpet01ItemUniqueIndex = 10114;
    private const string Closet01MeshName = "Closet_01";
    private const string Closet01ItemUniqueName = "Placeable_Closet01";
    private const int Closet01ItemUniqueIndex = 10116;
    private const string SignWallSurfaceAccessUpMeshName = "SignWall_SurfaceAccessUp";
    private const string SignWallSurfaceAccessUpItemUniqueName = "Placeable_SignWallSurfaceAccessUp";
    private const int SignWallSurfaceAccessUpItemUniqueIndex = 10117;
    private const string TangaroaSign1MeshName = "TangaroaSign1";
    private const string TangaroaSign1ItemUniqueName = "Placeable_TangaroaSign1";
    private const int TangaroaSign1ItemUniqueIndex = 10118;
    private const string TangaroaPaperPile2MeshName = "TangaroaPaperPile2";
    private const string TangaroaPaperPile2ItemUniqueName = "Placeable_TangaroaPaperPile2";
    private const int TangaroaPaperPile2ItemUniqueIndex = 10119;
    private const string SignWallPlantationMeshName = "SignWall_Plantation";
    private const string SignWallPlantationItemUniqueName = "Placeable_SignWallPlantation";
    private const int SignWallPlantationItemUniqueIndex = 10120;
    private const string SignDoorPlantationMeshName = "SignDoor_Plantation";
    private const string SignDoorPlantationItemUniqueName = "Placeable_SignDoorPlantation";
    private const int SignDoorPlantationItemUniqueIndex = 10121;
    private const string Floorlamp01MeshName = "Floorlamp_01";
    private const string Floorlamp01ItemUniqueName = "Placeable_Floorlamp01";
    private const int Floorlamp01ItemUniqueIndex = 10122;
    private const string Plate01MeshName = "Plate_01";
    private const string Plate01ItemUniqueName = "Placeable_Plate01";
    private const int Plate01ItemUniqueIndex = 10123;
    private const string BedDoubleMeshName = "BedDouble";
    private const string BedDoubleItemUniqueName = "Placeable_BedDouble";
    private const int BedDoubleItemUniqueIndex = 10124;
    private const string BedSmallMeshName = "BedSmall_Mesh";
    private const string BedSmallItemUniqueName = "Placeable_BedSmall";
    private const int BedSmallItemUniqueIndex = 10125;
    private const string MetalpipeMeshName = "Metalpipe_01";
    private const string MetalpipeItemUniqueName = "Placeable_Metalpipe01";
    private const int MetalpipeItemUniqueIndex = 10126;
    private const string ArmchairMeshName = "Armchair_01";
    private const string ArmchairItemUniqueName = "Placeable_Armchair01";
    private const int ArmchairItemUniqueIndex = 10127;
    private const string BookstopMeshName = "Bookstop_01";
    private const string BookstopItemUniqueName = "Placeable_Bookstop01";
    private const int BookstopItemUniqueIndex = 10128;
    private const string CabinetMeshName = "Cabinet_01";
    private const string CabinetItemUniqueName = "Placeable_Cabinet01";
    private const int CabinetItemUniqueIndex = 10129;
    private const string CanvasBlankMeshName = "Canvas_Blank";
    private const string CanvasBlankItemUniqueName = "Placeable_CanvasBlank";
    private const int CanvasBlankItemUniqueIndex = 10130;
    private const string CanvasStackBlankMeshName = "CanvasStack_Blank";
    private const string CanvasStackBlankItemUniqueName = "Placeable_CanvasStackBlank";
    private const int CanvasStackBlankItemUniqueIndex = 10131;
    private const string Flowerpot01MeshName = "Flowerpot_01";
    private const string Flowerpot01ItemUniqueName = "Placeable_Flowerpot01";
    private const int Flowerpot01ItemUniqueIndex = 10132;
    private const string TangaroaNumberSign5MeshName = "TangaroaNumberSign5";
    private const string TangaroaNumberSign5ItemUniqueName = "Placeable_TangaroaNumberSign5";
    private const int TangaroaNumberSign5ItemUniqueIndex = 10133;
    private const string Trashcan01MeshName = "Trashcan_01";
    private const string Trashcan01ItemUniqueName = "Placeable_Trashcan01";
    private const int Trashcan01ItemUniqueIndex = 10134;
    private const string OutOfOrderPosterMeshName = "OutOfOrderPoster";
    private const string OutOfOrderPosterItemUniqueName = "Placeable_OutOfOrderPoster";
    private const int OutOfOrderPosterItemUniqueIndex = 10135;
    private const string PresentPile1MeshName = "PresentPile1";
    private const string PresentPile1ItemUniqueName = "Placeable_PresentPile1";
    private const int PresentPile1ItemUniqueIndex = 10136;
    private const string TangaroaBathroomSinkMeshName = "Tangaroa_Bathroom_Sink";
    private const string TangaroaBathroomSinkItemUniqueName = "Placeable_TangaroaBathroomSink";
    private const int TangaroaBathroomSinkItemUniqueIndex = 10137;
    private const string Metalpipe02MeshName = "Metalpipe_02";
    private const string Metalpipe02ItemUniqueName = "Placeable_Metalpipe02";
    private const int Metalpipe02ItemUniqueIndex = 10138;
    private const string Painting1MeshName = "Painting1";
    private const string Painting1ItemUniqueName = "Placeable_Painting1";
    private const int Painting1ItemUniqueIndex = 10139;
    private const string Painting2MeshName = "Painting2";
    private const string Painting2ItemUniqueName = "Placeable_Painting2";
    private const int Painting2ItemUniqueIndex = 10140;
    private const string Painting3MeshName = "Painting3";
    private const string Painting3ItemUniqueName = "Placeable_Painting3";
    private const int Painting3ItemUniqueIndex = 10141;
    private const string TangaroaSign2MeshName = "TangaroaSign2";
    private const string TangaroaSign2ItemUniqueName = "Placeable_TangaroaSign2";
    private const int TangaroaSign2ItemUniqueIndex = 10142;
    private const string MousepadMeshName = "Mousepad";
    private const string MousepadItemUniqueName = "Placeable_Mousepad";
    private const int MousepadItemUniqueIndex = 10143;
    private const string Hanger01MeshName = "Hanger_01";
    private const string Hanger01ItemUniqueName = "Placeable_Hanger01";
    private const int Hanger01ItemUniqueIndex = 10144;
    private const string SoySackPile1MeshName = "SoySackPile4";
    private const string SoySackPile1ItemUniqueName = "Placeable_SoySackPile1";
    private const int SoySackPile1ItemUniqueIndex = 10145;
    private const string Bowl01MeshName = "Bowl_01";
    private const string Bowl01ItemUniqueName = "Placeable_Bowl01";
    private const int Bowl01ItemUniqueIndex = 10146;
    private const string Mirror01MeshName = "Mirror_01";
    private const string Mirror01ItemUniqueName = "Placeable_Mirror01";
    private const int Mirror01ItemUniqueIndex = 10147;
    private const string TangaroaNumberSign8MeshName = "TangaroaNumberSign8";
    private const string TangaroaNumberSign8ItemUniqueName = "Placeable_TangaroaNumberSign8";
    private const int TangaroaNumberSign8ItemUniqueIndex = 10148;
    private const string TangaroaNumberSign2MeshName = "TangaroaNumberSign2";
    private const string TangaroaNumberSign2ItemUniqueName = "Placeable_TangaroaNumberSign2";
    private const int TangaroaNumberSign2ItemUniqueIndex = 10149;
    private const string PottedSoyPlant1MeshName = "PottedSoyPlant1";
    private const string PottedSoyPlant1ItemUniqueName = "Placeable_PottedSoyPlant1";
    private const int PottedSoyPlant1ItemUniqueIndex = 10150;
    private const string TangaroaPlantsSmallPlant1MeshName = "TangaroaPlants_SmallPlant1";
    private const string TangaroaPlantsSmallPlant1ItemUniqueName = "Placeable_TangaroaPlantsSmallPlant1";
    private const int TangaroaPlantsSmallPlant1ItemUniqueIndex = 10151;
    private const string Shelf01MeshName = "Shelf_01";
    private const string Shelf01ItemUniqueName = "Placeable_Shelf01";
    private const int Shelf01ItemUniqueIndex = 10152;
    private const string TrayHolder1MeshName = "TrayHolder1";
    private const string TrayHolder1ItemUniqueName = "Placeable_TrayHolder1";
    private const int TrayHolder1ItemUniqueIndex = 10153;
    private const string Ashtray01MeshName = "Ashtray_01";
    private const string Ashtray01ItemUniqueName = "Placeable_Ashtray01";
    private const int Ashtray01ItemUniqueIndex = 10154;
    private const string SoySackPile3MeshName = "SoySackPile3";
    private const string SoySackPile3ItemUniqueName = "Placeable_SoySackPile3";
    private const int SoySackPile3ItemUniqueIndex = 10155;
    private const string ShelfMeshName = "Shelf";
    private const string ShelfItemUniqueName = "Placeable_Shelf";
    private const int ShelfItemUniqueIndex = 10156;
    private const string VentMeshName = "Vent";
    private const string VentItemUniqueName = "Placeable_Vent";
    private const int VentItemUniqueIndex = 10159;
    private const string RtCameraBaseMeshName = "RT_CameraBase";
    private const string RtCameraBaseItemUniqueName = "Placeable_RtCameraBase";
    private const int RtCameraBaseItemUniqueIndex = 10160;
    private const string ClipboardMeshName = "RT_Clipboard";
    private const string ClipboardItemUniqueName = "Placeable_RtClipboard";
    private const int ClipboardItemUniqueIndex = 10161;
    private const string FireEstinguisherMeshName = "Fire Estinguisher";
    private const string FireEstinguisherItemUniqueName = "Placeable_FireEstinguisher";
    private const int FireEstinguisherItemUniqueIndex = 10162;
    private const string BoxSmallMeshName = "Box Small";
    private const string BoxSmallItemUniqueName = "Placeable_BoxSmall";
    private const int BoxSmallItemUniqueIndex = 10163;
    private const string CubeMeshName = "Lamp";
    private const string CubeItemUniqueName = "Placeable_Cube";
    private const int CubeItemUniqueIndex = 10164;
    private const string SharedAssets6TableMeshName = "Sharedassets6_Table137";
    private const string SharedAssets6TableItemUniqueName = "Placeable_SharedAssets6Table";
    private const int SharedAssets6TableItemUniqueIndex = 10165;
    private const string SatelliteDishMeshName = "Parabol_3";
    private const string SatelliteDishItemUniqueName = "SatelliteDish";
    private const int SatelliteDishItemUniqueIndex = 10166;
    private const string RTNotepadMeshName = "RT_Notepad";
    private const string RTNotepadItemUniqueName = "RT_Notepad";
    private const int RTNotepadItemUniqueIndex = 10167;
    private const string RTWallVentMeshName = "RT_WallVent";
    private const string RTWallVentItemUniqueName = "RT_WallVent";
    private const int RTWallVentItemUniqueIndex = 10168;
    private const string RTCommRadioMeshName = "RT_CommRadio";
    private const string RTCommRadioItemUniqueName = "RT_CommRadio";
    private const int RTCommRadioItemUniqueIndex = 10169;
    private const string RTPowerBoxMeshName = "RT_PowerBox";
    private const string RTPowerBoxItemUniqueName = "RTPowerBox";
    private const int RTPowerBoxItemUniqueIndex = 10170;

    
    //NEW -------------------
    
    //END NEW ---------------




    private const float PlacementColliderScale = 0.9f;
    private const float BalloonPlacementColliderScale = 0.78f;
    private const float MinPlacementColliderAxis = 0.02f;
    private static readonly Vector3 DefaultColliderAxisScale = Vector3.one;
    private static readonly Vector3 RugColliderAxisScale = new Vector3(0.80f, 0.5f, 0.80f);
    private static readonly Vector3 WhiteBoardColliderAxisScale = new Vector3(0.85f, 0.85f, 0.18f);
    private static readonly Vector3 ThinWallDecorColliderAxisScale = new Vector3(0.90f, 0.90f, 0.10f);
    private static Vector3 EaselPaintingSocketLocalPosition = new Vector3(0.023f, 1.23f, 0.276f);
    private static readonly Quaternion EaselPaintingSocketRotationCompensation = Quaternion.Euler(-90f, 180f, 0f);
    // User-facing easel tilt/yaw/roll values for debug commands.
    private static Vector3 EaselPaintingSocketFriendlyEuler = new Vector3(-15f, 0f, 0f);
    private static Quaternion EaselPaintingSocketLocalRotation = ComposeEaselSocketLocalRotation(new Vector3(-15f, 0f, 0f));
    private static string EaselSocketDebugTargetUniqueName = Painting2ItemUniqueName;
    private static readonly Vector3 EaselPaintingSocketColliderSize = new Vector3(0.80f, 1.00f, 0.08f);
    private static readonly Vector3 BalloonVisualOffset = new Vector3(0f, 0f, -0.02f);
    private static readonly Vector3 RTPowerBoxVisualOffset = new Vector3(0f, 0f, -0.15f);
    private static readonly Vector3 RTWallVentVisualOffset = new Vector3(0f, 0f, -0.08f);
    private static readonly Vector3 WallLampVisualOffset = new Vector3(0f, 0f, -0.10f);
    private static readonly Vector3 StairWallLampVisualOffset = new Vector3(0f, 0f, -0.01f);
    private static readonly Vector3 LoadingBayVisualOffset = new Vector3(0f, 0f, -0.05f);
    private static readonly Vector3 SurfaceAccessVisualOffset = new Vector3(0f, 0f, -0.01f);
    private static readonly Vector3 ToiletVisualOffset = new Vector3(0f, 0f, -0.38f);
    private static readonly Vector3 CanvasBlankVisualOffset = new Vector3(0f, 0f, -0.037f);
    private static readonly Vector3 Hanger01VisualOffset = new Vector3(0f, 0f, -0.08f);
    private static readonly Vector3 TowelHangerVisualOffset = new Vector3(0f, 0f, -0.008f);
    private static readonly Vector3 VentVisualOffset = new Vector3(0f, 0f, -0.3f);
    private static readonly Vector3 ClipboardVisualOffset = new Vector3(0f, 0.013f, 0f);
    private static readonly Vector3 SharedAssets6TableVisualOffset = new Vector3(0f, -0.000000001f, 0f);
    private static readonly Quaternion FlippedVisualRotation = Quaternion.Euler(0f, 180f, 0f);
    private static readonly Quaternion ForwardVisualRotation = Quaternion.identity;
    private static readonly Quaternion IconFacingFlipRotation = Quaternion.Euler(0f, 180f, 0f);
    private static readonly Quaternion Mirror01VisualRotation = Quaternion.Euler(0f, 90f, 0f);
    private static readonly Quaternion WallLampVisualRotation = Quaternion.Euler(180f, -90f, -180f);
    private static readonly Quaternion ClockVisualRotation = Quaternion.Euler(90f, 0f, 180f);
    private static readonly Quaternion ToiletVisualRotation = Quaternion.Euler(0f, 0f, 0f);
    private static readonly Quaternion FireEstinguisherVisualRotation = Quaternion.Euler(-90f, 0f, 0f);
    private static readonly Quaternion TrashCanVisualRotation = Quaternion.Euler(-90f, 0f, 0f);
    private static readonly Quaternion VentVisualRotation = Quaternion.Euler(-90f, 0f, 180f);
    private static readonly Vector3 BalloonVisualScale = Vector3.one;

    private static readonly ModelDefinition[] ModelDefinitions =
    {
        // Wall Decorations & Mirrors
        new ModelDefinition(CustomItemUniqueName, CustomItemUniqueIndex, "Balloons", "Decorative balloons.", BalloonMeshName, FlippedVisualRotation, BalloonPlacementColliderScale, false, RBlockQuadType.quad_wall),
        new ModelDefinition(WallBalloons1ItemUniqueName, WallBalloons1ItemUniqueIndex, "Balloons", "Decorative balloons.", WallBalloons1MeshName, FlippedVisualRotation, BalloonPlacementColliderScale, false, RBlockQuadType.quad_wall),
        new ModelDefinition(MirrorItemUniqueName, MirrorItemUniqueIndex, "Wall Mirror", "A bathroom mirror for decorating walls.", MirrorMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(Mirror01ItemUniqueName, Mirror01ItemUniqueIndex, "Wall Mirror", "A decorative wall mirror.", Mirror01MeshName, Mirror01VisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(ShowerItemUniqueName, ShowerItemUniqueIndex, "Shower", "A decorative wall-mounted shower fixture.", ShowerMeshName, ForwardVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(WhiteBoardItemUniqueName, WhiteBoardItemUniqueIndex, "Whiteboard", "A decorative whiteboard.", WhiteBoardMeshName, FlippedVisualRotation, 0.75f, true, WhiteBoardColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(OutOfOrderPosterItemUniqueName, OutOfOrderPosterItemUniqueIndex, "Out Of Order Poster", "A decorative wall poster.", OutOfOrderPosterMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(CanvasBlankItemUniqueName, CanvasBlankItemUniqueIndex, "Canvas", "A blank canvas for decoration.", CanvasBlankMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(CanvasStackBlankItemUniqueName, CanvasStackBlankItemUniqueIndex, "Canvas Stack", "A stack of blank canvases for decorating spaces.", CanvasStackBlankMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(WallDecor02ItemUniqueName, WallDecor02ItemUniqueIndex, "Wall Decor", "A decorative wall piece.", WallDecor02MeshName, WallLampVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(WallDecor03ItemUniqueName, WallDecor03ItemUniqueIndex, "Wall Decor", "A decorative wall piece.", WallDecor03MeshName, WallLampVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(Painting1ItemUniqueName, Painting1ItemUniqueIndex, "Painting", "A decorative wall and easel painting.", Painting1MeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(Painting2ItemUniqueName, Painting2ItemUniqueIndex, "Painting", "A decorative wall and easel painting.", Painting2MeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(Painting3ItemUniqueName, Painting3ItemUniqueIndex, "Painting", "A decorative wall and easel painting.", Painting3MeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(Easel1ItemUniqueName, Easel1ItemUniqueIndex, "Easel", "You can put paintings on it.", Easel1MeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(ChartBoardItemUniqueName, ChartBoardItemUniqueIndex, "Chart Board", "A decorative chart board for floors.", ChartBoardMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Hanger01ItemUniqueName, Hanger01ItemUniqueIndex, "Hanger", "A decorative wall hanger.", Hanger01MeshName, ForwardVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),

        // Signs & Number Signs
        new ModelDefinition(SignWallLoadingBayUpItemUniqueName, SignWallLoadingBayUpItemUniqueIndex, "Loading Bay Sign", "A decorative wall sign.", SignWallLoadingBayUpMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(SignWallSurfaceAccessUpItemUniqueName, SignWallSurfaceAccessUpItemUniqueIndex, "Surface Access Sign", "A decorative wall sign.", SignWallSurfaceAccessUpMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(SignWallPlantationItemUniqueName, SignWallPlantationItemUniqueIndex, "Plantation Sign", "A decorative wall sign.", SignWallPlantationMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(SignDoorSurfaceAccessItemUniqueName, SignDoorSurfaceAccessItemUniqueIndex, "Surface Access Sign", "A decorative wall sign.", SignDoorSurfaceAccessMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(SignDoorStorageAreaItemUniqueName, SignDoorStorageAreaItemUniqueIndex, "Storage Area Sign", "A decorative wall sign.", SignDoorStorageAreaMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(SignDoorCafeteriaItemUniqueName, SignDoorCafeteriaItemUniqueIndex, "Cafeteria Sign", "A decorative wall sign.", SignDoorCafeteriaMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(SignDoorPlantationItemUniqueName, SignDoorPlantationItemUniqueIndex, "Plantation Sign", "A decorative wall sign.", SignDoorPlantationMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaSign1ItemUniqueName, TangaroaSign1ItemUniqueIndex, "Tangaroa Sign", "A decorative wall sign.", TangaroaSign1MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaSign2ItemUniqueName, TangaroaSign2ItemUniqueIndex, "Tangaroa Sign", "A decorative wall sign.", TangaroaSign2MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaSign3ItemUniqueName, TangaroaSign3ItemUniqueIndex, "Tangaroa Sign", "A decorative wall sign.", TangaroaSign3MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaSign4ItemUniqueName, TangaroaSign4ItemUniqueIndex, "High Voltage Sign", "A decorative wall sign.", TangaroaSign4MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaSign5ItemUniqueName, TangaroaSign5ItemUniqueIndex, "Tangaroa Sign", "A decorative wall sign.", TangaroaSign5MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign1ItemUniqueName, TangaroaNumberSign1ItemUniqueIndex, "Number 1 Sign", "A sign with the number 1 on it.", TangaroaNumberSign1MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign2ItemUniqueName, TangaroaNumberSign2ItemUniqueIndex, "Number 2 Sign", "A sign with the number 2 on it.", TangaroaNumberSign2MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign3ItemUniqueName, TangaroaNumberSign3ItemUniqueIndex, "Number 3 Sign", "A sign with the number 3 on it.", TangaroaNumberSign3MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign4ItemUniqueName, TangaroaNumberSign4ItemUniqueIndex, "Number 4 Sign", "A sign with the number 4 on it.", TangaroaNumberSign4MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign5ItemUniqueName, TangaroaNumberSign5ItemUniqueIndex, "Number 5 Sign", "A sign with the number 5 on it.", TangaroaNumberSign5MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign6ItemUniqueName, TangaroaNumberSign6ItemUniqueIndex, "Number 6 Sign", "A sign with the number 6 on it.", TangaroaNumberSign6MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaNumberSign8ItemUniqueName, TangaroaNumberSign8ItemUniqueIndex, "Number 8 Sign", "A sign with the number 8 on it.", TangaroaNumberSign8MeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),

        // Lighting
        new ModelDefinition(TablelampItemUniqueName, TablelampItemUniqueIndex, "Table Lamp", "A decorative table lamp for floors and tables.", TablelampMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Floorlamp01ItemUniqueName, Floorlamp01ItemUniqueIndex, "Floor Lamp", "A decorative floor lamp.", Floorlamp01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(WallLampsItemUniqueName, WallLampsItemUniqueIndex, "Wall Lamp", "A decorative wall lamp fixture.", WallLampsMeshName, WallLampVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(StairwellWallLampItemUniqueName, StairwellWallLampItemUniqueIndex, "Wall Lamp", "A decorative wall-mounted lamp.", StairwellWallLampMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(Lamppost01ItemUniqueName, Lamppost01ItemUniqueIndex, "Lamp Post", "A decorative lamp post.", Lamppost01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Lamppost02ItemUniqueName, Lamppost02ItemUniqueIndex, "Lamp Post", "A decorative lamp post.", Lamppost02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),

        // Shelving & Storage
        new ModelDefinition(Closet01ItemUniqueName, Closet01ItemUniqueIndex, "Closet", "A decorative closet for floors.", Closet01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(CabinetItemUniqueName, CabinetItemUniqueIndex, "Cabinet", "A decorative cabinet.", CabinetMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Drawer01ItemUniqueName, Drawer01ItemUniqueIndex, "Drawer", "A decorative drawer.", Drawer01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Container02ItemUniqueName, Container02ItemUniqueIndex, "Skip", "A decorative Skip.", Container02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Shelf01ItemUniqueName, Shelf01ItemUniqueIndex, "Metal Shelf", "A decorative shelf for floors.", Shelf01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Shelf02ItemUniqueName, Shelf02ItemUniqueIndex, "Metal Shelf", "A decorative shelf for floors.", Shelf02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Shelf03ItemUniqueName, Shelf03ItemUniqueIndex, "Metal Shelf", "A decorative shelf for floors.", Shelf03MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(ShelfItemUniqueName, ShelfItemUniqueIndex, "Shelf", "A decorative shelf for floors.", ShelfMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(BookstopItemUniqueName, BookstopItemUniqueIndex, "Bookstop", "A decorative bookstop.", BookstopMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Books
        new ModelDefinition(BookItemUniqueName, BookItemUniqueIndex, "Book", "A decorative book for floors and shelves.", BookMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book01ItemUniqueName, Book01ItemUniqueIndex, "Book 01", "A decorative book for tables and floors.", Book01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book02ItemUniqueName, Book02ItemUniqueIndex, "Book 02", "A decorative book for tables and floors.", Book02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book03ItemUniqueName, Book03ItemUniqueIndex, "Book 03", "A decorative book for tables and floors.", Book03MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book04ItemUniqueName, Book04ItemUniqueIndex, "Book 04", "A decorative book for tables and floors.", Book04MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book06ItemUniqueName, Book06ItemUniqueIndex, "Book 06", "A decorative book for tables and floors.", Book06MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book07ItemUniqueName, Book07ItemUniqueIndex, "Book 07", "A decorative book for tables and floors.", Book07MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Book08ItemUniqueName, Book08ItemUniqueIndex, "Book 08", "A decorative book for tables and floors.", Book08MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Rugs & Carpets
        new ModelDefinition(Carpet01ItemUniqueName, Carpet01ItemUniqueIndex, "Carpet", "A decorative carpet.", Carpet01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Carpet02ItemUniqueName, Carpet02ItemUniqueIndex, "Carpet", "A decorative carpet.", Carpet02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Carpet03ItemUniqueName, Carpet03ItemUniqueIndex, "Carpet", "A decorative carpet.", Carpet03MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(RugItemUniqueName, RugItemUniqueIndex, "Rug", "A decorative rug for floors.", RugMeshName, ForwardVisualRotation, 0.72f, false, RugColliderAxisScale, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Rug01ItemUniqueName, Rug01ItemUniqueIndex, "Rug", "A decorative rug for floors.", Rug01MeshName, ForwardVisualRotation, 0.72f, false, RugColliderAxisScale, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Rug03ItemUniqueName, Rug03ItemUniqueIndex, "Rug", "A decorative rug for floors.", Rug03MeshName, ForwardVisualRotation, 0.72f, false, RugColliderAxisScale, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Rug04ItemUniqueName, Rug04ItemUniqueIndex, "Rug", "A decorative rug for floors.", Rug04MeshName, ForwardVisualRotation, 0.72f, false, RugColliderAxisScale, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Paint Products
        new ModelDefinition(PaintBrushJarItemUniqueName, PaintBrushJarItemUniqueIndex, "Paint Brush Jar", "A decorative paint brush jar for tables.", PaintBrushJarMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PaintJarBlackItemUniqueName, PaintJarBlackItemUniqueIndex, "Black Paint Jar", "A decorative black paint jar for tables.", PaintJarBlackMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PaintJarBlueItemUniqueName, PaintJarBlueItemUniqueIndex, "Blue Paint Jar", "A decorative blue paint jar for tables.", PaintJarBlueMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PaintJarGreenItemUniqueName, PaintJarGreenItemUniqueIndex, "Green Paint Jar", "A decorative green paint jar for tables.", PaintJarGreenMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PaintJarRedItemUniqueName, PaintJarRedItemUniqueIndex, "Red Paint Jar", "A decorative red paint jar for tables.", PaintJarRedMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PaintJarWhiteItemUniqueName, PaintJarWhiteItemUniqueIndex, "Paint Jar White", "A decorative white paint jar for tables.", PaintJarWhiteMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Furniture - Seating
        new ModelDefinition(ArmchairItemUniqueName, ArmchairItemUniqueIndex, "Armchair", "A decorative armchair.", ArmchairMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Dinnerchair01ItemUniqueName, Dinnerchair01ItemUniqueIndex, "Dinner Chair", "A decorative dining chair.", Dinnerchair01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(OfficeChairItemUniqueName, OfficeChairItemUniqueIndex, "Office Chair", "A decorative office chair.", OfficeChairMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(OutdoorBenchItemUniqueName, OutdoorBenchItemUniqueIndex, "Outdoor Bench", "A decorative outdoor bench.", OutdoorBenchMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SofaItemUniqueName, SofaItemUniqueIndex, "Sofa", "A decorative sofa.", SofaMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(SofaU01ItemUniqueName, SofaU01ItemUniqueIndex, "U-Sofa", "A U-shaped sofa.", SofaU01MeshName, ForwardVisualRotation, PlacementColliderScale, false, DefaultColliderAxisScale, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TangaroaBenchItemUniqueName, TangaroaBenchItemUniqueIndex, "Tangaroa Bench", "A decorative bench.", TangaroaBenchMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),

        // Furniture - Beds
        new ModelDefinition(BedDoubleItemUniqueName, BedDoubleItemUniqueIndex, "Double Bed", "A comfortable double bed.", BedDoubleMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(BedSmallItemUniqueName, BedSmallItemUniqueIndex, "Small Bed", "A decorative small bed.", BedSmallMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),

        // Furniture - Tables
        new ModelDefinition(BedsideTableItemUniqueName, BedsideTableItemUniqueIndex, "Bedside Table", "A bedside table for floors.", BedsideTableMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Bedsidetable01ItemUniqueName, Bedsidetable01ItemUniqueIndex, "Bedside Table", "A decorative bedside table for floors.", Bedsidetable01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(OutdoorTableItemUniqueName, OutdoorTableItemUniqueIndex, "Outdoor Table", "A decorative outdoor table.", OutdoorTableMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(PoolTableItemUniqueName, PoolTableItemUniqueIndex, "Pool Table", "Pool, But where are the balls?", PoolTableMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TableCommon01ItemUniqueName, TableCommon01ItemUniqueIndex, "Table", "A decorative table.", TableCommon01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SharedAssets6TableItemUniqueName, SharedAssets6TableItemUniqueIndex, "Table", "A decorative table from RadioTower.", SharedAssets6TableMeshName, "sharedassets6.assets", 137, FireEstinguisherVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TableDining01ItemUniqueName, TableDining01ItemUniqueIndex, "Dining Table", "A dining table.", TableDining01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),

        // Kitchen & Dining
        new ModelDefinition(CoffeeCupItemUniqueName, CoffeeCupItemUniqueIndex, "Tangaroa Coffee Cup", "A decorative coffee cup featuring the Tangaroa logo .", CoffeeCupMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(CoffeeMachineItemUniqueName, CoffeeMachineItemUniqueIndex, "Coffee Machine", "A coffee machine for tables and floors.", CoffeeMachineMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaKitchenBench5SinkItemUniqueName, TangaroaKitchenBench5SinkItemUniqueIndex, "Kitchen Sink", "A kitchen sink counter.", TangaroaKitchenBench5SinkMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TangaroaKitchenCuttingBoardItemUniqueName, TangaroaKitchenCuttingBoardItemUniqueIndex, "Kitchen Cutting Board", "A decorative cutting board", TangaroaKitchenCuttingBoardMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaKitchenFridgeItemUniqueName, TangaroaKitchenFridgeItemUniqueIndex, "Kitchen Fridge", "A decorative kitchen fridge.", TangaroaKitchenFridgeMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TangaroaKitchenIslandWineShelfItemUniqueName, TangaroaKitchenIslandWineShelfItemUniqueIndex, "Kitchen Wine Shelf", "Where is my wine?!?", TangaroaKitchenIslandWineShelfMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Bowl01ItemUniqueName, Bowl01ItemUniqueIndex, "Bowl", "A decorative bowl for tables and floors.", Bowl01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Bowl02ItemUniqueName, Bowl02ItemUniqueIndex, "Bowl", "A decorative bowl for tables and floors.", Bowl02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Plate01ItemUniqueName, Plate01ItemUniqueIndex, "Plate", "A decorative plate for tables and floors.", Plate01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TrayItemUniqueName, TrayItemUniqueIndex, "Tray", "A tray for tables and floors.", TrayMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TrayHolder1ItemUniqueName, TrayHolder1ItemUniqueIndex, "Tray Holder", "A tray holder for floors and foundations.", TrayHolder1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Ashtray01ItemUniqueName, Ashtray01ItemUniqueIndex, "Ashtray", "A decorative ashtray for tables, floors, and foundations.", Ashtray01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_table, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Utensolholder01ItemUniqueName, Utensolholder01ItemUniqueIndex, "Utensil Holder", "A decorative utensil holder.", Utensolholder01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Soap01ItemUniqueName, Soap01ItemUniqueIndex, "Soap", "don't put in your mouth..", Soap01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Bottles01ItemUniqueName, Bottles01ItemUniqueIndex, "Soap Bottle", "Mmm. Soap..", Bottles01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(WBottle02ItemUniqueName, WBottle02ItemUniqueIndex, "Whisky Bottle", "A Whisky bottle for tables and floors.", WBottle02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(CakeItemUniqueName, CakeItemUniqueIndex, "Old Cake", "A decorative cake that can be placed on floors and tables.", CakeMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Textiles & Bath
        new ModelDefinition(RolledTowelsItemUniqueName, RolledTowelsItemUniqueIndex, "Rolled Towels", "A rolled towel stack.", RolledTowelsMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Toiletpaper01ItemUniqueName, Toiletpaper01ItemUniqueIndex, "Toilet Paper", "A toilet paper roll..", Toiletpaper01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Toilet01ItemUniqueName, Toilet01ItemUniqueIndex, "Toilet", "A decorative wall-mounted toilet.", Toilet01MeshName, ToiletVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(TowelHangerItemUniqueName, TowelHangerItemUniqueIndex, "Towel Hanger", "A decorative towel hanger for walls.", TowelHangerMeshName, ForwardVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(Tub01ItemUniqueName, Tub01ItemUniqueIndex, "Tub", "A decorative bathtub.", Tub01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TangaroaBathroomSinkItemUniqueName, TangaroaBathroomSinkItemUniqueIndex, "Bathroom Sink", "A decorative bathroom sink.", TangaroaBathroomSinkMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),

        // Vases & Decorative Items
        new ModelDefinition(Flowerpot01ItemUniqueName, Flowerpot01ItemUniqueIndex, "Pot", "A decorative pot for floors and tables.", Flowerpot01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Flowerpot03ItemUniqueName, Flowerpot03ItemUniqueIndex, "Pot", "A decorative pot for floors and tables.", Flowerpot03MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Vases01ItemUniqueName, Vases01ItemUniqueIndex, "Vase", "A decorative vase.", Vases01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Vases02ItemUniqueName, Vases02ItemUniqueIndex, "Vase", "A decorative vase.", Vases02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(Vases03ItemUniqueName, Vases03ItemUniqueIndex, "Vase", "A decorative vase.", Vases03MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PartyHatItemUniqueName, PartyHatItemUniqueIndex, "Party Hat", "can't wear it though.. :(", PartyHatMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PresentOpenItemUniqueName, PresentOpenItemUniqueIndex, "Open Present", "A decorative open present for tables and floors.", PresentOpenMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PresentPile1ItemUniqueName, PresentPile1ItemUniqueIndex, "Present Pile", "A decorative pile of presents.", PresentPile1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PresentPile2ItemUniqueName, PresentPile2ItemUniqueIndex, "Present Pile", "A decorative pile of presents.", PresentPile2MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Plants & Vegetation
        new ModelDefinition(HedgeBush1ItemUniqueName, HedgeBush1ItemUniqueIndex, "Small Hedge", "A decorative hedge for floors.", HedgeBush1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(HedgeBush2ItemUniqueName, HedgeBush2ItemUniqueIndex, "Large Hedge", "A decorative hedge for floors.", HedgeBush2MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(PotBush1ItemUniqueName, PotBush1ItemUniqueIndex, "Small Potted Bush", "A decorative potted bush for floors.", PotBush1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(PotBush2ItemUniqueName, PotBush2ItemUniqueIndex, "Large Potted Bush", "A decorative potted bush for floors.", PotBush2MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(PottedSoyPlant1ItemUniqueName, PottedSoyPlant1ItemUniqueIndex, "Potted Soy Plant", "A decorative potted soy plant for tables and floors.", PottedSoyPlant1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(PottedSoyPlant2ItemUniqueName, PottedSoyPlant2ItemUniqueIndex, "Potted Soy Plant", "A decorative potted soy plant for tables and floors.", PottedSoyPlant2MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaPlantsSmallPlant1ItemUniqueName, TangaroaPlantsSmallPlant1ItemUniqueIndex, "Small Plant", "A decorative small plant for tables and floors.", TangaroaPlantsSmallPlant1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaPlantsSmallPlant2ItemUniqueName, TangaroaPlantsSmallPlant2ItemUniqueIndex, "Small Plant", "A decorative small plant for tables and floors.", TangaroaPlantsSmallPlant2MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaPlantsSmallPlant3ItemUniqueName, TangaroaPlantsSmallPlant3ItemUniqueIndex, "Small Plant", "A decorative small plant for tables and floors.", TangaroaPlantsSmallPlant3MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Papers & Soft Goods
        new ModelDefinition(SoySackItemUniqueName, SoySackItemUniqueIndex, "Soy Sack", "Yummy...", SoySackMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(SoySackPile1ItemUniqueName, SoySackPile1ItemUniqueIndex, "Soy Sack Pile", "A decorative soy sack pile.", SoySackPile1MeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SoySackPile2ItemUniqueName, SoySackPile2ItemUniqueIndex, "Soy Sack Pile", "A decorative soy sack pile.", SoySackPile2MeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SoySackPile3ItemUniqueName, SoySackPile3ItemUniqueIndex, "Soy Sack Pile", "A decorative soy sack pile.", SoySackPile3MeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(TangaroaPaperPile1ItemUniqueName, TangaroaPaperPile1ItemUniqueIndex, "Paper Pile", "A decorative paper pile for tables and floors.", TangaroaPaperPile1MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaPaperPile2ItemUniqueName, TangaroaPaperPile2ItemUniqueIndex, "Paper Pile", "A decorative paper pile for tables and floors.", TangaroaPaperPile2MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaPaperPile3ItemUniqueName, TangaroaPaperPile3ItemUniqueIndex, "Paper Pile", "A decorative paper pile for tables and floors.", TangaroaPaperPile3MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(RTNotepadItemUniqueName, RTNotepadItemUniqueIndex, "Notepad", "A decorative notepad for tables and floors.", RTNotepadMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),

        // Outdoor & Utility
        new ModelDefinition(Firehydrant01ItemUniqueName, Firehydrant01ItemUniqueIndex, "Fire Hydrant", "A decorative fire hydrant.", Firehydrant01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(RestaFenceItemUniqueName, RestaFenceItemUniqueIndex, "Metal Fence", "A decorative fence.", RestaFenceMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SunshadeGroundItemUniqueName, SunshadeGroundItemUniqueIndex, "Umbrella", "A decorative ground umbrella.", SunshadeGroundMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SunshadeWallItemUniqueName, SunshadeWallItemUniqueIndex, "Wall Sunshade", "A decorative wall-mounted sunshade.", SunshadeWallMeshName, FlippedVisualRotation, PlacementColliderScale, true, ThinWallDecorColliderAxisScale, RBlockQuadType.quad_wall),
        new ModelDefinition(TangaroaBucketItemUniqueName, TangaroaBucketItemUniqueIndex, "Tangaroa Bucket", "A decorative bucket.", TangaroaBucketMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaHoseWheelItemUniqueName, TangaroaHoseWheelItemUniqueIndex, "Hose Wheel", "A decorative hose.", TangaroaHoseWheelMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(TangaroaWheelBarrowItemUniqueName, TangaroaWheelBarrowItemUniqueIndex, "Wheel Barrow", "A decorative wheel barrow.", TangaroaWheelBarrowMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(VentilationBoxItemUniqueName, VentilationBoxItemUniqueIndex, "Ventilation Box", "A wall-mounted ventilation box.", VentilationBoxMeshName, ForwardVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(VentItemUniqueName, VentItemUniqueIndex, "Vent", "A wall-mounted vent grille.", VentMeshName, VentVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(MetalpipeItemUniqueName, MetalpipeItemUniqueIndex, "Metal Pipe", "A decorative metal pipe.", MetalpipeMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Metalpipe02ItemUniqueName, Metalpipe02ItemUniqueIndex, "Metal Pipe", "A decorative metal pipe.", Metalpipe02MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(Trashcan01ItemUniqueName, Trashcan01ItemUniqueIndex, "Trashcan", "A decorative trashcan.", Trashcan01MeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(SatelliteDishItemUniqueName, SatelliteDishItemUniqueIndex, "Satellite dish", "A decorative Satellite dish.", SatelliteDishMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation),
        new ModelDefinition(RTWallVentItemUniqueName, RTWallVentItemUniqueIndex, "Vent", "A wall-mounted vent grille.", RTWallVentMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(RTPowerBoxItemUniqueName, RTPowerBoxItemUniqueIndex, "Power Box", "A wall-mounted power box.", RTPowerBoxMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),

        // Clocks
        new ModelDefinition(Clock01ItemUniqueName, Clock01ItemUniqueIndex, "Clock", "A decorative wall clock.", Clock01MeshName, ClockVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(Clock02ItemUniqueName, Clock02ItemUniqueIndex, "Clock", "A decorative wall clock.", Clock02MeshName, ClockVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),

        // Technology & Office Items
        new ModelDefinition(ComputerChassisItemUniqueName, ComputerChassisItemUniqueIndex, "Computer", "A decorative computer.", ComputerChassisMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(FlatScreenItemUniqueName, FlatScreenItemUniqueIndex, "Flat Screen Monitor", "A decorative flat screen Monitor.", FlatScreenMeshName, FlippedVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(KeyboardItemUniqueName, KeyboardItemUniqueIndex, "Keyboard", "A decorative computer keyboard for tables and floors.", KeyboardMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(MouseItemUniqueName, MouseItemUniqueIndex, "Mouse", "A decorative computer mouse for tables and floors.", MouseMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(MousepadItemUniqueName, MousepadItemUniqueIndex, "Mousepad", "A decorative mousepad for tables and floors.", MousepadMeshName, ForwardVisualRotation, PlacementColliderScale, false, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(ClipboardItemUniqueName, ClipboardItemUniqueIndex, "Clipboard", "A clipboard for offices.", ClipboardMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(FireEstinguisherItemUniqueName, FireEstinguisherItemUniqueIndex, "Fire Estinguisher", "A Fire Estinguisher for offices.", FireEstinguisherMeshName, FireEstinguisherVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(RtCameraBaseItemUniqueName, RtCameraBaseItemUniqueIndex, "Security Camera", "A wall-mounted security camera.", RtCameraBaseMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_wall),
        new ModelDefinition(CubeItemUniqueName, CubeItemUniqueIndex, "Lamp", "Eww... Kinda rusty", CubeMeshName, FireEstinguisherVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(BoxSmallItemUniqueName, BoxSmallItemUniqueIndex, "A box", "Too bad you dont have a cat.. or do you....?", BoxSmallMeshName, FireEstinguisherVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),
        new ModelDefinition(RTCommRadioItemUniqueName, RTCommRadioItemUniqueIndex, "Communication Radio", "A 1980s style communication radio.", RTCommRadioMeshName, FlippedVisualRotation, PlacementColliderScale, true, RBlockQuadType.quad_floor, RBlockQuadType.quad_foundation, RBlockQuadType.quad_table),



        //New - To sort --------------------
    };

    private const string ExplicitSourceCacheTag = "_explicit";

    private static readonly Dictionary<string, ModelDefinition> ModelsByUniqueName = ModelDefinitions.ToDictionary(model => model.UniqueName);
    private static readonly HashSet<string> CustomUniqueNames = new HashSet<string>(ModelDefinitions.Select(model => model.UniqueName));
    private static readonly HashSet<string> RequiredMeshNames = new HashSet<string>(ModelDefinitions.Where(model => !model.HasExplicitSource).Select(model => model.MeshName));
    private static readonly ILookup<string, ModelDefinition> ExplicitSourceModelsByAssetFile =
        ModelDefinitions.Where(model => model.HasExplicitSource).ToLookup(model => model.SourceAssetFile);
    private static readonly HashSet<string> CompositeMeshNames = new HashSet<string>
    {
        TangaroaKitchenBench5SinkMeshName,
        TangaroaKitchenFridgeMeshName,
        TangaroaKitchenIslandWineShelfMeshName,
        RtCameraBaseMeshName
    };
    private static readonly Dictionary<string, MeshVisualData> PreloadedVisuals = new Dictionary<string, MeshVisualData>();
    private static readonly Dictionary<MeshIdentity, MeshVisualData> PreloadedVisualsByIdentity = new Dictionary<MeshIdentity, MeshVisualData>();
    private static readonly Dictionary<string, Sprite> GeneratedIconsByUniqueName = new Dictionary<string, Sprite>();
    private static readonly List<Texture2D> GeneratedIconTextures = new List<Texture2D>();
    private static readonly List<Sprite> GeneratedIconSprites = new List<Sprite>();
    private static readonly string[] EaselSocketAcceptedUniqueNames =
    {
        Painting1ItemUniqueName,
        Painting2ItemUniqueName,
        Painting3ItemUniqueName,
        CanvasBlankItemUniqueName
    };
    private struct EaselSocketProfile
    {
        public Vector3 Position;
        public Vector3 FriendlyEuler;

        public EaselSocketProfile(Vector3 position, Vector3 friendlyEuler)
        {
            Position = position;
            FriendlyEuler = friendlyEuler;
        }
    }
}