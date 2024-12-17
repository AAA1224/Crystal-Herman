using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine;

public class Options_Controller
{
    public Button preset1, preset2, preset3;
    public Toggle preset1Toggle, preset2Toggle, preset3Toggle;
    public static bool presetFlag1, presetFlag2, presetFlag3;
    //Preset 1 (Personal Finance)=================================
    private bool preset1FlatTax = false;
    private bool preset1ProgTax = true;
    private int preset1ProgTaxUpper = 30;
    private int preset1BaseTaxRate = 20;
    private int preset1BaseTaxAmount = 100;
    private int preset1MaintCost = 50;
    private int preset1CrystalIncome = 10;
    private bool preset1WaterManualIntro = true;
    private int preset1WaterTurnIntro = 4;
    private float preset1DisasterSev = .5f;
    private int preset1DisasterFreq = 40;
    //Preset 2 (Community Finance)================================
    private bool preset2FlatTax = true;
    private bool preset2ProgTax = false;
    private int preset2BaseTaxRate = 0;
    private int preset2BaseTaxAmount = 100;
    private int preset2MaintCost = 70;
    private int preset2CrystalIncome = 10;
    private bool preset2WaterManualIntro = true;
    private int preset2WaterTurnIntro = 5;
    private float preset2DisasterSev = .5f;
    private int preset2DisasterFreq = 40;
    //Preset 3 ===================================================
    private bool preset3FlatTax = true;
    private bool preset3ProgTax = false;
    private int preset3BaseTaxRate = 0;
    private int preset3BaseTaxAmount = 150;
    private int preset3MaintCost = 60;
    private int preset3CrystalIncome = 10;
    private bool preset3WaterManualIntro = true;
    private int preset3WaterTurnIntro = 3;
    private float preset3DisasterSev = .5f;
    private int preset3DisasterFreq = 40;

    //Options Menu settings=======================================

    //Is the flat tax option enabled?
    public static bool flatTax = true;
    public static bool progTax = false;
    //The upper limit for the progressive tax
    public static int progressiveTaxUpper = 10;
    //The base tax percentage
    public static int baseTaxRate = 10;
    //The base tax amount
    public static int baseTaxAmount = 100;

    //The base maintenance cost
    public static int baseMaintCost = 50;

    //The base income for water crystals
    public static int baseWaterIncome = 10;
    public static bool manualIntroWater = false;
    public static int waterIntroTurn = 1;

    //Severity of the disaster low .5, med 1, high 2
    public static float severityFactor = 1;
    //Frequency of the disaster, default , low 10%, med 25%, high 40%
    public static int frequencyFactor = 0;
    //Chance to get a player match for disasters
    public static int disasterMatchChance = 80;

    //Show/Hide panel for all players
    public static bool showPanel = false;

    //============================================================


    //Options menu UI elements ===================================
    public GameObject optionsPanel;
    public Button closeButton, openButton;

    //Tax UI options
    public Toggle flatToggle;
    public Button flatEnable;
    public Toggle progToggle;
    public Button progEnable;
    public Slider progSlider;
    public Text progText, progMinText;
    public InputField baseTaxField;
    public InputField baseTaxAmountField;

    //Maintanence UI options
    public InputField baseMaintField;

    //Water UI options
    public InputField waterIncomeField;
    public Toggle waterManualIntro;
    public Button waterEnable;
    public InputField waterTurn;

    //Disater options
    public Dropdown disasterSev;
    public Dropdown disasterFreq;
    public InputField disasterChance;

    //Mayor Panel toggle
    public Toggle panelToggle;
    public Button panelButton;

    //============================================================
}
