﻿using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSIDCertificates.MC_04
{
    public partial class MC04Form : Form
    {
        DataTable dtRBDS08Readings = new DataTable();
        DataTable dtDeviceDetails = new DataTable();
        DataTable dtTempHumidity = new DataTable();
        public MC04Form()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }

        private void MC04Form_Load(object sender, EventArgs e)
        {
            try
            {
                dtRBDS08Readings = GetRBDS08ReadingsDetails();
                dtDeviceDetails = GetRBDS08Details();
                dtTempHumidity = GetTempHumidity();

                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsRBDS08Readings = new ReportDataSource("dsRBDS08Readings", dtRBDS08Readings);
                ReportDataSource dsRBDS08Details = new ReportDataSource("dsRBDS08Details", dtDeviceDetails);
                ReportDataSource dsTempHumidity = new ReportDataSource("dsTempHumidity", dtTempHumidity);

                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsRBDS08Readings);
                this.reportViewer1.LocalReport.DataSources.Add(dsRBDS08Details);
                this.reportViewer1.LocalReport.DataSources.Add(dsTempHumidity);

                this.reportViewer1.RefreshReport();
            }
            catch(Exception ex)
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }
        private DataTable GetRBDS08ReadingsDetails()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select RBDS08_OPSideNominalValue, RBDS08_OPSideMeasuredValue, RBDS08_OPSideError, " +
                        "RBDS08_DRSideNominalValue, RBDS08_DRSideMeasuredValue, RBDS08_DRSideError " +
                        "FROM tblRBDS08ReadingsMaster ", con);
                    //cmd.ExecuteNonQuery();

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsRBDS08Readings = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtRBDS08Readings);
                            return dtRBDS08Readings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the RB#4-DS-08-Readings data.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
        private DataTable GetRBDS08Details()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select Machine_Name, Cycle_Name, Device_Name, Device_ControlID, " +
                        "Device_Make, Device_Model, " +
                        "CONCAT(Device_LHRangeCombined, ' ' , Device_RangeUnit) AS Device_LHRangeCombined, " +
                        "CONCAT(Device_LC, ' ' , Device_LCUnit) AS Device_LCCombined, " +
                        "CONCAT(Device_Accuracy, ' ' , Device_AccuracyUnit) AS Device_AccuracyCombined, " +
                        "CONCAT(Device_JudgementSpecs, ' ' , Device_JudgementUnit) AS Device_JudgementCombined," +
                        "CONCAT(Device_LHOPRangeCombined, ' ' , Device_OperatingRangeUnit) AS Device_OPRangeCombined, " +
                        "Inst_Name, Inst_ControlIDCombined FROM tblDeviceMaster " +
                        "join tblMachineMaster on tblDeviceMaster.Machine_ID = tblMachineMaster.Machine_ID " +
                        "join tblCycleMaster on tblDeviceMaster.Cycle_ID = tblCycleMaster.Cycle_ID " +
                        "join tblDeviceInstMaster on tblDeviceMaster.Device_ID = tblDeviceInstMaster.Device_ID " +
                        "join tblInstrumentMaster on tblDeviceInstMaster.Inst_ID = tblInstrumentMaster.Inst_ID " +
                        "where tblDeviceMaster.Device_ControlID=@Device_ControlID ", con);
                    cmd.Parameters.AddWithValue("@Device_ControlID", "RB#4-DS-08");

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsRBDS08Details = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtDeviceDetails);
                            return dtDeviceDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the RB#4-DS-08-Details.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
        private DataTable GetTempHumidity()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select Top 1 CONCAT(THReading_Temperature, ' °C ', THReading_Humidity, ' % ') AS TempHumidityCombined " +
                        "from tblTHReadingMaster", con);

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsTempHumidity = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtTempHumidity);
                            return dtTempHumidity;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the RB#4-DS-08-TempHumidity Details.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
    }
}
