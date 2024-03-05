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

namespace BSIDCertificates.PG
{
    public partial class PGForm : Form
    {
        DataTable dtBB8PGReadings = new DataTable();
        DataTable dtBB8Details = new DataTable();
        DataTable dtTempHumidity = new DataTable();
        public PGForm()
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
        private void PGForm_Load(object sender, EventArgs e)
        {
            try
            {
                dtBB8PGReadings = GetBB8PGReadingsDetails();
                dtBB8Details = GetBB8PGDetails();
                dtTempHumidity = GetTempHumidity();

                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsBB8PGReadings = new ReportDataSource("dsBB8PGReadings", dtBB8PGReadings);
                ReportDataSource dsBB8PGDetails = new ReportDataSource("dsBB8PGDetails", dtBB8Details);
                ReportDataSource dsTempHumidity = new ReportDataSource("dsTempHumidity", dtTempHumidity);

                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsBB8PGReadings);
                this.reportViewer1.LocalReport.DataSources.Add(dsBB8PGDetails);
                this.reportViewer1.LocalReport.DataSources.Add(dsTempHumidity);

                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }
        private DataTable GetBB8PGReadingsDetails()
        {
            try
            {
                string constr = @"Data Source=SG2NWPLS19SQL-v06.mssql.shr.prod.sin2.secureserver.net; Initial Catalog=BridgestoneCal; Persist Security Info=True;User ID=bsid_admin;Password=BSID@2023";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select BB8PGReadings_InspectionPoints, BB8PGReadings_BeforeIncreasingError, BB8PGReadings_BeforeDecreasingError, " +
                        "BB8PGReadings_AfterIncreasingError, BB8PGReadings_AfterDecreasingError " +
                        "FROM tblBB8PGReadingsMaster " +
                        "ORDER BY CAST(BB8PGReadings_InspectionPoints AS DECIMAL(18,2)) ", con);
                    //cmd.ExecuteNonQuery();

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBB8PGReadings = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBB8PGReadings);
                            return dtBB8PGReadings;
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
        private DataTable GetBB8PGDetails()
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
                    cmd.Parameters.AddWithValue("@Device_ControlID", "BB#8-PG-08");

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (BridgestoneCalDataSet dsBB8PGDetails = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBB8Details);
                            return dtBB8Details;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BB8PG Details.");
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
                MessageBox.Show("Oops. Something went wrong while fetching the BB8PG TempHumidity Details.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
    }
}
