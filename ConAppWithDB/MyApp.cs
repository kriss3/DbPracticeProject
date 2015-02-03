using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServerCe.Client;

using System.Reflection;

namespace ConAppWithDB
{
	class MyApp
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Start...Calling Run Method...\n");
			Console.WriteLine("1: \tUsing DataReader with sdf DB file");
			Console.WriteLine("2: \tUsing DataSet with sdf DB file");
			Console.WriteLine("3: \tUsing DataSet with external access to MS Sql\n");
			Console.Write("Your choice: ");
			string result = Console.ReadLine();

			int value;
			bool myChoice = int.TryParse(result, out value);
			switch (value)
			{
				case 1:
					RunWithDataReader();
					break;
				case 2:
					RunWithDataSet();
					break;
				case 3:
					RunWithDataSetPayables();
					break;
				default:
					Console.WriteLine("Wrong choice...");
					break;
			}
			Console.ReadLine();
		}

		public static void RunWithDataReader()
		{
			Console.WriteLine("\nStarting method: " + MethodBase.GetCurrentMethod().Name + "\n");

			SqlCeConnection conn = null;

			try
			{
				//this should be in App.config and read using ConfigurationManager
				conn = new SqlCeConnection("Data Source = ksTestDb.sdf; Password = 'test'");

				string query = "Select FirstName from [ks.Customer]";
				SqlCeCommand command = new SqlCeCommand(query, conn);
				conn.Open();

				SqlCeDataReader myReader = command.ExecuteReader();

				while (myReader.Read() == true)
				{
					String fn = myReader.GetString(0);
					Console.WriteLine(fn);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				conn.Close();
			}
		}

		public static void RunWithDataSet()
		{
			Console.WriteLine("\nStarting method: " + MethodBase.GetCurrentMethod().Name);
			//need to find a better way of retrieving connString
			String connString = "Data Source = ksTestDb.sdf; Password = krisland";
			SqlCeConnection conn = new SqlCeConnection(connString);

			try
			{
				conn.Open();

				String query = "Select * from [ks.Customer]";
				SqlCeCommand command = new SqlCeCommand(query, conn);

				SqlCeDataAdapter dataAd = new SqlCeDataAdapter(command);

				//Both below lines work fine, need to find out the difference
				//DataSet myDs = new DataSet();
				KsTestDbDataSet myDs = new KsTestDbDataSet();

				dataAd.Fill(myDs, "ks.Customer");
				
				DataTable dt = myDs.Tables["ks.Customer"];
				foreach (DataRow row in dt.Rows)
				{
					foreach (DataColumn column in dt.Columns)
					{
						Console.WriteLine(row[column]);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				conn.Close();
			}
		}

		public static void RunWithDataSetPayables()
		{
			Console.WriteLine("\nStarting method: " + MethodBase.GetCurrentMethod().Name);

			SqlConnection pConn = new SqlConnection("Data Source=KRISDELL;Initial Catalog=Payables;Integrated Security=True");

			string pQuery = @"Select Name, Address1 From Vendors";
			SqlCommand pCommand = new SqlCommand(pQuery, pConn);
			SqlDataAdapter da = new SqlDataAdapter(pCommand);
			
			DataSet dsPayables = new DataSet();
			da.Fill(dsPayables, "Vendors");

			DataTable pDataTable = dsPayables.Tables["Vendors"];

			foreach (DataRow row in pDataTable.Rows)
			{
				Console.WriteLine();
				foreach (DataColumn col in pDataTable.Columns)
				{
					Console.Write(row[col].ToString() + " |  ");
				}
			}
		}
	}
}
