#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using Game;


public class MyExcelEditor : Editor
{

    [MenuItem("ExcelEditor/test")] 
    static void test()
    {
        Excel xls = new Excel();
        ExcelTable table = new ExcelTable();
        table.TableName = "test";
        string outputPath = ExcelEditor.DocsPath + "/Test2.xlsx";
        xls.Tables.Add(table);
        xls.Tables[0].SetValue(1, 1, "1");
        xls.Tables[0].SetValue(1, 2, "2");
        xls.Tables[0].SetValue(2, 1, "3");
        xls.Tables[0].SetValue(2, 2, "4");
        xls.ShowLog();
        ExcelHelper.SaveExcel(xls, outputPath);
    }

    [MenuItem("ExcelEditor/LoadXls")] 
    static void LoadXls()
    {
//        string path = Application.dataPath + "/Test/Test4.xlsx";
        string path = ExcelEditor.DocsPath + "/数值平衡.xlsx";
        Excel xls =  ExcelHelper.LoadExcel(path);
        xls.ShowLog();
        Debug.Log(xls.Tables[0].TableName);
    }

    [MenuItem("ExcelEditor/WriteXls")] 
    static void WriteXls()
    {
        Excel xls = new Excel();
        ExcelTable table = new ExcelTable();
        table.TableName = "test";
//        string outputPath = Application.dataPath + "/Test/Test2.xlsx";
        string outputPath = ExcelEditor.DocsPath + "/Test.xlsx";
        xls.Tables.Add(table);
        xls.Tables[0].SetValue(1, 1, "字段1");
        xls.Tables[0].SetValue(1, 2, "字段2");
        xls.Tables[0].SetValue(1, 3, "字段3");
        for (int i = 2; i <= 11; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                xls.Tables[0].SetValue(i, j, Random.Range(1000,100000).ToString());
            }
        }
        xls.ShowLog();
        ExcelTable table2 = new ExcelTable();
        table2.TableName = "标签2";
        xls.Tables.Add(table2);
        xls.Tables[1].SetValue(1, 1, Random.Range(1000,100000).ToString());
        xls.Tables[1].SetValue(1, 2, "测试");
        xls.Tables[1].SetValue(1, 3, "我勒个去我勒个去我勒个去我勒个去");
        xls.Tables[1].SetValue(4, 3, "我在这里");
        xls.ShowLog();
        ExcelHelper.SaveExcel(xls, outputPath);
    }

    [MenuItem("ExcelEditor/MathBattles")]
    static void MathBattles() {
        string path = ExcelEditor.DocsPath + "/数值平衡.xlsx";
        Excel xls =  ExcelHelper.LoadExcel(path);
        ExcelTable table = xls.Tables[0];
        Debug.Log(table.TableName + "计算开始。。。");
        Debug.Log(table.NumberOfColumns + "," + table.NumberOfRows);
        List<string> areaNames = new List<string>() { };
        List<List<RoleData>> friends = new List<List<RoleData>>();
        List<List<RoleData>> enemys = new List<List<RoleData>>();
        string areaName;
        RoleData friend;
        RoleData enemy;
        for (int i = 1; i < table.NumberOfRows; i++) {
            if (string.IsNullOrEmpty(table.GetValue(i, 2).ToString())) {
                continue;
            }
            areaName = table.GetValue(i, 1).ToString();
            if (!string.IsNullOrEmpty(areaName)) {
                areaNames.Add(areaName);
                friends.Add(new List<RoleData>());
                enemys.Add(new List<RoleData>());
            } else {
                friend = new RoleData();
                friend.IsKnight = false;
                friend.Id = table.GetValue(i, 2).ToString();
                friend.Name = table.GetValue(i, 3).ToString();
                friend.Lv = int.Parse(table.GetValue(i, 4).ToString());
                friend.DifLv4HP = int.Parse(table.GetValue(i, 7).ToString());
                friend.DifLv4PhysicsAttack = int.Parse(table.GetValue(i, 9).ToString());
                friend.DifLv4PhysicsDefense = int.Parse(table.GetValue(i, 11).ToString());
                friend.DifLv4MagicAttack = int.Parse(table.GetValue(i, 13).ToString());
                friend.DifLv4MagicDefense = int.Parse(table.GetValue(i, 15).ToString());
                friend.DifLv4Dodge = int.Parse(table.GetValue(i, 17).ToString());
                friend.MakeJsonToModel();
                friend.Init();
                friends[friends.Count - 1].Add(friend);
//                Debug.Log(JsonManager.GetInstance().SerializeObject(friend));
                enemy = new RoleData();
                enemy.IsKnight = false;
                enemy.Id = table.GetValue(i, 21).ToString();
                enemy.Name = table.GetValue(i, 22).ToString();
                enemy.Lv = int.Parse(table.GetValue(i, 23).ToString());
                enemy.DifLv4HP = int.Parse(table.GetValue(i, 26).ToString());
                enemy.DifLv4PhysicsAttack = int.Parse(table.GetValue(i, 28).ToString());
                enemy.DifLv4PhysicsDefense = int.Parse(table.GetValue(i, 30).ToString());
                enemy.DifLv4MagicAttack = int.Parse(table.GetValue(i, 32).ToString());
                enemy.DifLv4MagicDefense = int.Parse(table.GetValue(i, 34).ToString());
                enemy.DifLv4Dodge = int.Parse(table.GetValue(i, 36).ToString());
                enemy.MakeJsonToModel();
                enemy.Init();
//                Debug.Log(JsonManager.GetInstance().SerializeObject(enemy));
                enemys[enemys.Count - 1].Add(enemy);
            }
        }

        Debug.Log(table.TableName + "计算结束，请查看原表。");
    }
}
#endif
