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
        DataTable dtBB8PGReadingsTable = new DataTable();
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
                dtBB8PGReadingsTable = GetBB8PGReadingsTableDetails();

                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsBB8PGReadingsTable = new ReportDataSource("dsBB8PGReadingsTable", dtBB8PGReadingsTable);

                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsBB8PGReadingsTable);

                this.reportViewer1.RefreshReport();
            }
            catch(Exception ex) 
            {
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
            }
        }
        private DataTable GetBB8PGReadingsTableDetails()
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
                        using (BridgestoneCalDataSet dsBB8PGReadingsTable = new BridgestoneCalDataSet())
                        {
                            sda.Fill(dtBB8PGReadingsTable);
                            return dtBB8PGReadingsTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops. Something went wrong while fetching the BB8PGReadings Table data.");
                LogError _LE = new LogError();
                _LE.LogDetails(ex);
                return null;
            }
        }
    }
}
