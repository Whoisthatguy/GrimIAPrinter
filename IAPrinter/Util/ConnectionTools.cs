using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace IAPrinter.Util
{
	public static class ConnectionTools
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConnectionTools));

		public static string ModifyDatabaseConnectionString(
			string configConnectionStringName,
			string dataSource = "")
		{
			try
			{
				string oldConString = ConfigurationManager.ConnectionStrings[configConnectionStringName].ConnectionString;
				var entityCnxStringBuilder = new EntityConnectionStringBuilder
					(oldConString);

				if (!string.IsNullOrEmpty(dataSource))
					entityCnxStringBuilder.ProviderConnectionString = $"data source={dataSource}";
				return entityCnxStringBuilder.ConnectionString;
			}
			catch (Exception ex)
			{
				log.Error("Error while modifying db string:", ex);
			}
			return null;
		}

		public static void ChangeConnectionString(string connectionString, string configConnectionStringName)
		{
			try
			{
				string providerName = ConfigurationManager.ConnectionStrings[configConnectionStringName].ProviderName;
				var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
				connectionStringsSection.ConnectionStrings[configConnectionStringName].ConnectionString = connectionString;
				connectionStringsSection.ConnectionStrings[configConnectionStringName].ProviderName = providerName;
				config.Save();
				ConfigurationManager.RefreshSection("connectionStrings");
			}
			catch (Exception ex)
			{
				log.Error("Error while saving connection string:", ex);
			}
		}
	}
}
