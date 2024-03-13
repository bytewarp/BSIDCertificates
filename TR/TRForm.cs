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

namespace BSIDCertificates.TR
{
    public partial class TRForm : Form
    {
        DataTable dtBB8TRReadings = new DataTable();
        DataTable dtDeviceDetails = new DataTable();
        DataTable dtTempHumidity = new DataTable();
        public TRForm()
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
        private void TRForm_Load(object sender, EventArgs e)
        {
            try
            {
                dtBB8TRReadings = GetBB8TRReadingsDetails();
                dtDeviceDetails = GetBB8TRDetails();
                dtTempHumidity = GetTempHumidity();

                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsBB8TRReadings = new ReportDataSource("dsBB8TRReadings", dtBB8TRReadings);
                ReportDataSource dsBB8TRDetails = new ReportDataSource("dsBB8TRDetails", dtDeviceDetails);
                ReportDataSource dsTempHumidity = new ReportDataSource("dsTempHumidity", dtTempHumidity);

                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsBB8TRReadings);
                this.reportViewer1.LocalReport.DataSources.Add(dsBB8TRDetails);
                this.reportViewer1.LocalReport.DataSources.Add(dsTempHumidity);

                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }
        private DataTable GetBB8TRReadingsDetails()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select BB8TRReadings_InputDCVoltage, BB8TRReadings_Channel1ChartReading, BB8TRReadings_Channel1Display, " +
                        "BB8TRReadings_Channel2ChartReading, BB8TRReadings_Channel2Display, " +
                        "BB8TRReadings_Channel3ChartReading, BB8TRReadings_Channel3Display, " +
                        "BB8TRReadings_Channel4ChartReading, BB8TRReadings_Channel4Display " +
                        "FROM tblBB8TRReadingsMaster ", con);
                    //cmd.ExecuteNonQuery();

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBB8TRReadings = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBB8TRReadings);
                            return dtBB8TRReadings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BB#8-TR-09-Readings data.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
        private DataTable GetBB8TRDetails()
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
                        "CONCAT(Device_JudgementSpecs, ' ' , Device_JudgementUnit) AS Device_JudgementCombined, " +
                        "Device_JudgementSpecs, CONCAT(Device_LHOPRangeCombined, ' ' , Device_OperatingRangeUnit) AS Device_OPRangeCombined, " +
                        "Inst_Name, Inst_ControlIDCombined FROM tblDeviceMaster " +
                        "join tblMachineMaster on tblDeviceMaster.Machine_ID = tblMachineMaster.Machine_ID " +
                        "join tblCycleMaster on tblDeviceMaster.Cycle_ID = tblCycleMaster.Cycle_ID " +
                        "join tblDeviceInstMaster on tblDeviceMaster.Device_ID = tblDeviceInstMaster.Device_ID " +
                        "join tblInstrumentMaster on tblDeviceInstMaster.Inst_ID = tblInstrumentMaster.Inst_ID " +
                        "where tblDeviceMaster.Device_ControlID=@Device_ControlID ", con);
                    cmd.Parameters.AddWithValue("@Device_ControlID", "BB#8-TR-09");

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBB8TRDetails = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtDeviceDetails);
                            return dtDeviceDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BB#8-TR-09-Details.");
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
                MessageBox.Show("Oops. Something went wrong while fetching the BB#8-TR-09-TempHumidity Details.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
    }
}
