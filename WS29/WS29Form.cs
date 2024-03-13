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

namespace BSIDCertificates.WS29
{
    public partial class WS29Form : Form
    {
        DataTable dtBQCT1WS29Readings = new DataTable();
        DataTable dtDeviceDetails = new DataTable();
        DataTable dtTempHumidity = new DataTable();
        public WS29Form()
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
        private void WS29Form_Load(object sender, EventArgs e)
        {
            try
            {
                dtBQCT1WS29Readings = GetBQCT1WS29ReadingsDetails();
                dtDeviceDetails = GetBQCT1WS29Details();
                dtTempHumidity = GetTempHumidity();

                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsBQCT1WS29Readings = new ReportDataSource("dsBQCT1WS29Readings", dtBQCT1WS29Readings);
                ReportDataSource dsBQCT1WS29Details = new ReportDataSource("dsBQCT1WS29Details", dtDeviceDetails);
                ReportDataSource dsTempHumidity = new ReportDataSource("dsTempHumidity", dtTempHumidity);

                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsBQCT1WS29Readings);
                this.reportViewer1.LocalReport.DataSources.Add(dsBQCT1WS29Details);
                this.reportViewer1.LocalReport.DataSources.Add(dsTempHumidity);

                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }
        private DataTable GetBQCT1WS29ReadingsDetails()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select BQCTWS29_NominalValue, BQCTWS29_MeasuredValue, BQCTWS29_Error " +
                        "FROM tblBQCTWS29ReadingsMaster " +
                        "ORDER BY CAST(BQCTWS29_NominalValue AS DECIMAL(18,2)) ", con);
                    //cmd.ExecuteNonQuery();

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBQCT1WS29Readings = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBQCT1WS29Readings);
                            return dtBQCT1WS29Readings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BQCT#1-WS-29-Readings data.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
        private DataTable GetBQCT1WS29Details()
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
                        "where tblDeviceMaster.Device_ControlID=@Device_ControlID and tblInstrumentMaster.Inst_Name=@Inst_Name ", con);
                    cmd.Parameters.AddWithValue("@Device_ControlID", "BQCT#1-WS-29");
                    cmd.Parameters.AddWithValue("@Inst_Name", "Weighing scale");

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBQCT1WS29Details = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtDeviceDetails);
                            return dtDeviceDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BQCT#1-WS-29-Details.");
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
                MessageBox.Show("Oops. Something went wrong while fetching the BQCT#1-WS-29-TempHumidity Details.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
    }
}
