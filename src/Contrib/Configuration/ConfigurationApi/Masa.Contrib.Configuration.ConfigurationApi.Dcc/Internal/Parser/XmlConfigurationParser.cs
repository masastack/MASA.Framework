// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Data;
using System.Text;

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal.Parser;

internal class XmlConfigurationParser
{
    public static string XmlToJson(string xmlStr)
    {
        var dataSet = XmlToDataSet(xmlStr);

        return DatasetToJson(dataSet);
    }

    public static DataSet XmlToDataSet(string xmlStr)
    {
        try
        {
            var ds = new DataSet();
            using var xmlStringReader = new StringReader(xmlStr);

            ds.ReadXml(xmlStringReader);

            return ds;
        }
        catch (Exception ex)
        {
            throw new FormatException("Xml character string invalid", ex);
        }
    }

    public static string DatasetToJson(DataSet ds)
    {
        var json = new StringBuilder();

        foreach (DataTable dt in ds.Tables)
        {
            json.Append("{\"");
            json.Append(dt.TableName);
            json.Append("\":");
            json.Append(DataTableToJson(dt));
            json.Append('}');
        }
        return json.ToString();
    }

    public static string DataTableToJson(DataTable table)
    {
        var jsonStringBuilder = new StringBuilder();
        if (table.Rows.Count > 0)
        {
            jsonStringBuilder.Append('[');
            for (int i = 0; i < table.Rows.Count; i++)
            {
                jsonStringBuilder.Append('{');
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (j < table.Columns.Count - 1)
                    {
                        jsonStringBuilder.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                    }
                    else if (j == table.Columns.Count - 1)
                    {
                        jsonStringBuilder.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                    }
                }
                if (i == table.Rows.Count - 1)
                {
                    jsonStringBuilder.Append('}');
                }
                else
                {
                    jsonStringBuilder.Append("},");
                }
            }
            jsonStringBuilder.Append(']');
        }
        return jsonStringBuilder.ToString();
    }
}
