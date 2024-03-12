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
                
                reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource dsBQCT1WS29Readings = new ReportDataSource("dsBQCT1WS29Readings", dtBQCT1WS29Readings);
                
                this.reportViewer1.LocalReport.DataSources.Clear();

                this.reportViewer1.LocalReport.DataSources.Add(dsBQCT1WS29Readings);
                
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
    }
}
