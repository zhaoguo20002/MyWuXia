#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
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
}
#endif
