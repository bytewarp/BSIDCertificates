using Microsoft.Reporting.WinForms;
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

namespace BSIDCertificates
{
    public partial class WSForm : Form
    {
        DataTable dtBB8WSReadings = new DataTable();
        DataTable dtBB8Details = new DataTable();
        DataTable dtTempHumidity = new DataTable();
        public WSForm()
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

        private void WSForm_Load(object sender, EventArgs e)
        {
            try
            {
                dtBB8WSReadings = GetBB8WSReadingsDetails();
                dtBB8Details = GetBB8WSDetails();
                dtTempHumidity = GetTempHumidity();

                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsBB8WSReadings = new ReportDataSource("dsBB8WSReadings", dtBB8WSReadings);
                ReportDataSource dsBB8WSDetails = new ReportDataSource("dsBB8WSDetails", dtBB8Details);
                ReportDataSource dsTempHumidity = new ReportDataSource("dsTempHumidity", dtTempHumidity);

                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsBB8WSReadings);
                this.reportViewer1.LocalReport.DataSources.Add(dsBB8WSDetails);
                this.reportViewer1.LocalReport.DataSources.Add(dsTempHumidity);

                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }
        private DataTable GetBB8WSReadingsDetails()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select BB8WSReadings_StdWtApplied, BB8WSReadings_BeforeAdjustmentDisplayIncreasing, BB8WSReadings_BeforeAdjustmentDisplayDecreasing, " +
                        "BB8WSReadings_BeforeAdjustmentMaxError, BB8WSReadings_AfterAdjustmentDisplayIncreasing, BB8WSReadings_AfterAdjustmentDisplayDecreasing, " +
                        "BB8WSReadings_AfterAdjustmentMaxError FROM tblBB8WSReadingsMaster " +
                        "ORDER BY CAST(BB8WSReadings_StdWtApplied AS DECIMAL(18,2)) ", con);
                    //cmd.ExecuteNonQuery();

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBB8WSReadings = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBB8WSReadings);
                            return dtBB8WSReadings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BB8PGReadings data.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
        private DataTable GetBB8WSDetails()
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
                        "Device_JudgementSpecs, CONCAT(Device_LHOPRangeCombined, ' ' , Device_OperatingRangeUnit) AS Device_OPRangeCombined, " +
                        "Inst_Name, Inst_ControlIDCombined FROM tblDeviceMaster " +
                        "join tblMachineMaster on tblDeviceMaster.Machine_ID = tblMachineMaster.Machine_ID " +
                        "join tblCycleMaster on tblDeviceMaster.Cycle_ID = tblCycleMaster.Cycle_ID " +
                        "join tblDeviceInstMaster on tblDeviceMaster.Device_ID = tblDeviceInstMaster.Device_ID " +
                        "join tblInstrumentMaster on tblDeviceInstMaster.Inst_ID = tblInstrumentMaster.Inst_ID " +
                        "where tblDeviceMaster.Device_ControlID=@Device_ControlID ", con);
                    cmd.Parameters.AddWithValue("@Device_ControlID", "BB#8-WS-01");

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBB8WSDetails = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBB8Details);
                            return dtBB8Details;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BB8WS Details.");
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
                MessageBox.Show("Oops. Something went wrong while fetching the BB8WS TempHumidity Details.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
    }
}
