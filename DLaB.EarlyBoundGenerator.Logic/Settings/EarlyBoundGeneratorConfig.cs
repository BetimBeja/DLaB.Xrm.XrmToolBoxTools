using Source.DLaB.Common;
using Source.DLaB.Common.VersionControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace DLaB.EarlyBoundGenerator.Settings
{
    /// <summary>
    /// POCO for EBG Settings
    /// </summary>
    [Serializable]
    [XmlType("Config")]
    [XmlRoot("Config")]
    public class EarlyBoundGeneratorConfig
    {
        #region Properties

        /// <summary>
        /// Use speech synthesizer to notify of code generation completion.
        /// </summary>
        [Category("Global")]
        [DisplayName("Audible Completion Notification")]
        [Description("Use speech synthesizer to notify of code generation completion.")]
        public bool AudibleCompletionNotification { get; set; }

        /// <summary>
        /// The CrmSvcUtil relative path.
        /// </summary>
        /// <value>
        /// The CrmSvcUtil relative path.
        /// </value>
        [Category("Global")]
        [DisplayName("CrmSvcUtil Relative Path")]
        [Description("The Path to the CrmSvcUtil.exe, relative to the CrmSvcUtil Realtive Root Path.  Defaults to using the CrmSvcUtil that comes by default.")]
        public string CrmSvcUtilRelativePath { get; set; }

        /// <summary>
        /// Specifies whether to include in the early bound class, the command line used to generate it.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include command line]; otherwise, <c>false</c>.
        /// </value>
        [Category("Global")]
        [DisplayName("Include Command Line")]
        [Description("Specifies whether to include in the early bound class, the command line used to generate it.")]
        public bool IncludeCommandLine { get; set; }

        /// <summary>
        /// Masks the password in the command line
        /// </summary>
        /// <value>
        ///   <c>true</c> if [mask password]; otherwise, <c>false</c>.
        /// </value>
        [Category("Global")]
        [DisplayName("Mask Password")]
        [Description("Masks the password in the outputted command line.")]
        public bool MaskPassword { get; set; }

        /// <summary>
        /// Gets or sets the last ran version.
        /// </summary>
        /// <value>
        /// The last ran version.
        /// </value>
        [Category("Meta")]
        [DisplayName("Settings Version")]
        [Description("The Settings File Version.")]
        [ReadOnly(true)]
        public string SettingsVersion { get; set; }
        /// <summary>
        /// The version of the EarlyBoundGeneratorPlugin
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [Category("Meta")]
        [DisplayName("Version")]
        [Description("Version of the Early Bound Generator.")]
        [ReadOnly(true)]
        public string Version { get; set; }
        /// <summary>
        /// Settings that will get written to the CrmSrvUtil.exe.config
        /// </summary>
        /// <value>
        /// The extension configuration.
        /// </value>
        [Category("CrmSvcUtil")]
        [DisplayName("Settings")]
        [Description("Settings that will get written to the CrmSrvUtil.exe.config.")]
        public ExtensionConfig ExtensionConfig { get; set; }
        /// <summary>
        /// These are the required commandline arguments that are passed to the CrmSrvUtil to correctly wire up the extensions in DLaB.CrmSvcUtilExtensions.
        /// </summary>
        /// <value>
        /// The extension arguments.
        /// </value>
        [Category("CrmSvcUtil")]
        [DisplayName("Extension Arguments")]
        [Description("These are the required commandline arguments that are passed to the CrmSrvUtil to correctly wire up the extensions in DLaB.CrmSvcUtilExtensions.")]
        public List<Argument> ExtensionArguments { get; set; }
        /// <summary>
        /// These are the commandline arguments that are passed to the CrmSrvUtil that can have varying values, depending on the user's preference.
        /// </summary>
        /// <value>
        /// The user arguments.
        /// </value>
        [Category("CrmSvcUtil")]
        [DisplayName("User Arguments")]
        [Description("Commandline arguments that are passed to the CrmSrvUtil that can have varying values, depending on the user's preference.")]
        public List<Argument> UserArguments { get; set; }
        /// <summary>
        /// Some actions are being created by MS that are not workflows, and don't show up in the list of actions for adding to whitelist/blacklist, but are getting genereated and causing errors.  This setting is used to manually add action names to the selected lists
        /// </summary>
        public string WorkflowlessActions { get; set; }

        #region NonSerialized Properties
        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool UseCrmOnline { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool UseConnectionString { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string Domain { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string UserName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string Password { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string RootPath { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool SupportsActions { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string Url { get; set; }

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public IEnumerable<Argument> CommandLineArguments => UserArguments.Union(ExtensionArguments);

        /// <summary>
        /// Path determined based on the Relative Path
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string CrmSvcUtilPath =>
            Directory.Exists(CrmSvcUtilRelativePath)
                ? CrmSvcUtilRelativePath
                : Path.Combine(CrmSvcUtilRelativeRootPath ?? Directory.GetCurrentDirectory(), CrmSvcUtilRelativePath);

        /// <summary>
        /// Set during Execution
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string CrmSvcUtilRelativeRootPath { get; set; }

        #region UserArguments Helpers

        /// <summary>
        /// Action output path
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string ActionOutPath
        {
            get { return GetUserArgument(CreationType.Actions, UserArgumentNames.Out).Value; }
            set { SetUserArgument(CreationType.Actions, UserArgumentNames.Out, value); }
        }

        /// <summary>
        /// Entity output path
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string EntityOutPath
        {
            get { return GetUserArgument(CreationType.Entities, UserArgumentNames.Out).Value; }
            set { SetUserArgument(CreationType.Entities, UserArgumentNames.Out, value); }
        }

        /// <summary>
        /// Namespace
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string Namespace
        {
            get { return GetUserArgument(CreationType.All, UserArgumentNames.Namespace).Value; }
            set { SetUserArgument(CreationType.All, UserArgumentNames.Namespace, value); }
        }

        /// <summary>
        /// Option set output path
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string OptionSetOutPath
        {
            get { return GetUserArgument(CreationType.OptionSets, UserArgumentNames.Out).Value; }
            set { SetUserArgument(CreationType.OptionSets, UserArgumentNames.Out, value); }
        }

        /// <summary>
        /// Name of the Service Context Created
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string ServiceContextName
        {
            get { return GetUserArgument(CreationType.Entities, UserArgumentNames.ServiceContextName).Value; }
            set { SetUserArgument(CreationType.Entities, UserArgumentNames.ServiceContextName, value); }
        }

        /// <summary>
        /// Controls if the SuppressGeneratedCodeAttribute is included
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public bool SuppressGeneratedCodeAttribute
        {
            get { return GetUserArgument(CreationType.All, UserArgumentNames.SuppressGeneratedCodeAttribute).Value == "true"; }
            set { SetUserArgument(CreationType.All, UserArgumentNames.SuppressGeneratedCodeAttribute, value ? "true" : "false"); }
        }

        #endregion // UserArguments Helpers

        #endregion // NonSerialized Properties

        #endregion // Properties       

        private EarlyBoundGeneratorConfig()
        {
            CrmSvcUtilRelativePath = Config.GetAppSettingOrDefault("CrmSvcUtilRelativePath", @"DLaB.EarlyBoundGenerator\crmsvcutil.exe");
            UseConnectionString = Config.GetAppSettingOrDefault("UseConnectionString", false);
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region Add Missing Default settings

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyBoundGeneratorConfig"/> class.
        /// </summary>
        /// <param name="poco">The poco.</param>
        private EarlyBoundGeneratorConfig(POCO.Config poco)
        {
            var @default = GetDefault();
            CrmSvcUtilRelativePath = poco.CrmSvcUtilRelativePath ?? @default.CrmSvcUtilRelativePath;
            RemoveObsoleteValues(poco, @default);

            AudibleCompletionNotification = poco.AudibleCompletionNotification ?? @default.AudibleCompletionNotification;
            IncludeCommandLine = poco.IncludeCommandLine ?? @default.IncludeCommandLine;
            MaskPassword = poco.MaskPassword ?? @default.MaskPassword;
            WorkflowlessActions = poco.WorkflowlessActions ?? @default.WorkflowlessActions;


            UpdateObsoleteSettings(poco, poco.ExtensionConfig, @default);

            ExtensionConfig = @default.ExtensionConfig;
            ExtensionConfig.SetPopulatedValues(poco.ExtensionConfig);

            ExtensionArguments = AddMissingArguments(poco.ExtensionArguments, @default.ExtensionArguments);
            UserArguments = AddMissingArguments(poco.UserArguments, @default.UserArguments);
            SettingsVersion = string.IsNullOrWhiteSpace(poco.Version) ? "0.0.0.0" : poco.Version;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static void UpdateObsoleteSettings(POCO.Config poco, POCO.ExtensionConfig pocoConfig, EarlyBoundGeneratorConfig @default)
        {
            var pocoVersion = new Version(poco.Version);
            if (pocoVersion < new Version("1.2016.6.1"))
            {
                // Storing of UnmappedProperties and EntityAttributeSpecified Names switched from Key,Value1,Value2|Key,Value1,Value2 to Key:Value1,Value2|Key:Value1,Value2
                // Also convert from a List to a HashSet
                pocoConfig.EntityAttributeSpecifiedNames = ConvertNonColonDelimitedDictionaryListToDictionaryHash(pocoConfig.EntityAttributeSpecifiedNames);
                pocoConfig.UnmappedProperties = ConvertNonColonDelimitedDictionaryListToDictionaryHash(pocoConfig.UnmappedProperties);
            }

            if (pocoVersion < new Version("1.2018.9.12"))
            {
                // Update the OptionSet codecustomization Argument Setting to use the new Generic Code Customization Service
                var oldValue = poco.ExtensionArguments.FirstOrDefault(
                    a => a.SettingType == CreationType.OptionSets
                         && a.Name == "codecustomization");
                var newValue = @default.ExtensionArguments.FirstOrDefault(
                    a => a.SettingType == CreationType.OptionSets
                         && a.Name == "codecustomization");
                if (oldValue != null
                    && newValue != null)
                {
                    poco.ExtensionArguments.Remove(oldValue);
                    poco.ExtensionArguments.Add(newValue);
                }
            }

            if (pocoVersion < new Version("1.2020.3.23"))
            {
                // Added new option to overwrite Option Set properties.  This is the desired default now, but don't break old generation settings.
                if (poco.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum == null)
                {
                    poco.ExtensionConfig.ReplaceOptionSetPropertiesWithEnum = false;
                }
            }

            if (pocoVersion < new Version("1.2020.10.1"))
            {
                // Issue #254 add invalid actions to blacklist.
                var invalidBlacklistItems = "RetrieveAppSettingList|RetrieveAppSetting|SaveAppSetting|msdyn_GetSIFeatureConfiguration".ToLower();
                if (string.IsNullOrWhiteSpace(poco.ExtensionConfig.ActionsToSkip))
                {
                    poco.ExtensionConfig.ActionsToSkip = invalidBlacklistItems;
                }
                else 
                {
                    poco.ExtensionConfig.ActionsToSkip += "|" + invalidBlacklistItems;
                }
            }

            if (pocoVersion < new Version("1.2020.12.18"))
            {
                // 12.18.2020 introduced Valueless parameters, but GenerateActions existed before as a null, need a boolean value to determine if it should be included
                var generateActions = poco.UserArguments.FirstOrDefault(a => a.Name == UserArgumentNames.GenerateActions && a.Value == null);
                if (generateActions != null)
                {
                    generateActions.Value = "true";
                    generateActions.Valueless = true;
                }
            }
        }

        private static string ConvertNonColonDelimitedDictionaryListToDictionaryHash(string oldValue)
        {
            if (oldValue == null)
            {
                return null;
            }
            var oldValues = Config.GetList<string>(Guid.NewGuid().ToString(), oldValue);
            var newValues = new Dictionary<string, HashSet<string>>();
            foreach (var entry in oldValues)
            {
                var hash = new HashSet<string>();
                var values = entry.Split(',');
                newValues.Add(values.First(), hash);
                foreach (var value in values.Skip(1).Where(v => !hash.Contains(v)))
                {
                    hash.Add(value);
                }
            }
            return Config.ToString(newValues);
        }

        private void RemoveObsoleteValues(POCO.Config poco, EarlyBoundGeneratorConfig @default)
        {
            if (CrmSvcUtilRelativePath == @"Plugins\CrmSvcUtil Ref\crmsvcutil.exe"
                || CrmSvcUtilRelativePath == @"CrmSvcUtil Ref\crmsvcutil.exe")
            {
                // 12.15.2016 XTB changed to use use the User directory, no plugin folder needed now
                // 3.12.2019 Nuget Stopped liking spaces in the CrmSvcUtilFolder.  Updated to be the plugin specific DLaB.EarlyBoundGenerator
                CrmSvcUtilRelativePath = @default.CrmSvcUtilRelativePath;
            }
            foreach (var value in poco.ExtensionArguments.Where(a => string.Equals(a.Value, "DLaB.CrmSvcUtilExtensions.Entity.OverridePropertyNames,DLaB.CrmSvcUtilExtensions", StringComparison.InvariantCultureIgnoreCase)).ToList())
            {
                // Pre 2.13.2016, this was the default value.  Replaced with a single naming service that both Entities and OptionSets can use
                poco.ExtensionArguments.Remove(value);
            }

            // Pre 2.13.2016, this was the default value.  Not Needed Anymore
            var old = "OpportunityProduct.OpportunityStateCode,opportunity_statuscode|" +
                      "OpportunityProduct.PricingErrorCode,qooi_pricingerrorcode|" +
                      "ResourceGroup.GroupTypeCode,constraintbasedgroup_grouptypecode";
            if (string.Equals(poco.ExtensionConfig.PropertyEnumMappings, old, StringComparison.InvariantCultureIgnoreCase) || string.Equals(poco.ExtensionConfig.PropertyEnumMappings, old + "|", StringComparison.InvariantCultureIgnoreCase))
            {
                poco.ExtensionConfig.PropertyEnumMappings = string.Empty;
            }
        }

        private string AddPipeDelimitedMissingDefaultValues(string value, string @default)
        {
            try
            {
                if (value == null || @default == null)
                {
                    return @default ?? value;
                }
                var splitValues = value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                var hash = new HashSet<string>(splitValues);
                splitValues.AddRange(@default.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).
                                              Where(key => !hash.Contains(key)));

                return Config.ToString(splitValues);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Processing config value: " + value, ex);
            }
        }

        private string AddMissingDictionaryHashDefaultValues(string value, string @default)
        {
            try
            {
                if (value == null || @default == null)
                {
                    return @default ?? value;
                }
                // Handle post serialization that saves this value off with newlines.
                value = value.Replace(Environment.NewLine, String.Empty);
                var values = Config.GetDictionaryHash<string, string>(Guid.NewGuid().ToString(), value);
                var defaultValues = Config.GetDictionaryHash<string, string>(Guid.NewGuid().ToString(), @default);

                foreach (var entry in defaultValues)
                {
                    HashSet<string> hash;
                    if (!values.TryGetValue(entry.Key, out hash))
                    {
                        hash = new HashSet<string>();
                        values[entry.Key] = hash;
                    }

                    foreach (var item in entry.Value.Where(v => !hash.Contains(v)))
                    {
                        hash.Add(item);
                    }
                }

                // All missing values have been added.  Join back Values
                return Config.ToString(values);
            }
            catch (Exception ex)
            {
                throw new Exception("Error Processing config value: " + value, ex);
            }
        }

        private List<Argument> AddMissingArguments(List<Argument> value, List<Argument> @default)
        {
            if (value == null || @default == null)
            {
                return value ?? @default ?? new List<Argument>();
            }
            value.AddRange(@default.Where(arg => !value.Any(a => a.SettingType == arg.SettingType && a.Name == arg.Name)));

            return value;
        }

        /// <summary>
        /// Gets the default config
        /// </summary>
        /// <returns></returns>
        public static EarlyBoundGeneratorConfig GetDefault()
        {

            var @default = new EarlyBoundGeneratorConfig
            {
                AudibleCompletionNotification = true,
                IncludeCommandLine = true,
                MaskPassword = true,
                ExtensionArguments = new List<Argument>(new[] {
                    // Actions
                    new Argument(CreationType.Actions, CrmSrvUtilService.CodeCustomization, "DLaB.CrmSvcUtilExtensions.Action.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, CrmSrvUtilService.CodeGenerationService, "DLaB.CrmSvcUtilExtensions.Action.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, CrmSrvUtilService.CodeWriterFilter, "DLaB.CrmSvcUtilExtensions.Action.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Actions, CrmSrvUtilService.MetadataProviderService, "DLaB.CrmSvcUtilExtensions.BaseMetadataProviderService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.CodeCustomization, "DLaB.CrmSvcUtilExtensions.Entity.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.CodeGenerationService, "DLaB.CrmSvcUtilExtensions.Entity.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.CodeWriterFilter, "DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.NamingService, "DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.Entities, CrmSrvUtilService.MetadataProviderService, "DLaB.CrmSvcUtilExtensions.Entity.MetadataProviderService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeCustomization, "DLaB.CrmSvcUtilExtensions.OptionSet.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeGenerationService, "DLaB.CrmSvcUtilExtensions.OptionSet.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.CodeWriterFilter, "DLaB.CrmSvcUtilExtensions.OptionSet.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.NamingService, "DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions"),
                    new Argument(CreationType.OptionSets, CrmSrvUtilService.MetadataProviderService, "DLaB.CrmSvcUtilExtensions.BaseMetadataProviderService,DLaB.CrmSvcUtilExtensions")
                }),
                ExtensionConfig = ExtensionConfig.GetDefault(),
                UserArguments = new List<Argument>(new[] {
                    new Argument(CreationType.Actions, UserArgumentNames.GenerateActions, "true"){ Valueless = true},
                    new Argument(CreationType.Actions, UserArgumentNames.Out,  @"EBG\Actions.cs"),
                    new Argument(CreationType.All, UserArgumentNames.Namespace, "CrmEarlyBound"),
                    new Argument(CreationType.All, UserArgumentNames.SuppressGeneratedCodeAttribute, "true"){ Valueless = true},
                    new Argument(CreationType.Entities, UserArgumentNames.Out, @"EBG\Entities.cs"),
                    new Argument(CreationType.Entities, UserArgumentNames.ServiceContextName, "CrmServiceContext"),
                    new Argument(CreationType.OptionSets, UserArgumentNames.Out,  @"EBG\OptionSets.cs")
                }),
                WorkflowlessActions = "RetrieveAppSettingList|RetrieveAppSetting|SaveAppSetting|msdyn_GetSIFeatureConfiguration"
            };
            @default.SettingsVersion = @default.Version;
            return @default;
        }

        #endregion // Add Missing Default settings

        /// <summary>
        /// Loads the Config from the given path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static EarlyBoundGeneratorConfig Load(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    var config = GetDefault();
                    return config;
                }

                var serializer = new XmlSerializer(typeof(POCO.Config));
                POCO.Config poco;
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    poco = (POCO.Config)serializer.Deserialize(fs);
                    fs.Close();
                }
                var settings = new EarlyBoundGeneratorConfig(poco);
                return settings;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured attempting to load Xml configuration: " + filePath, ex);
            }
        }

        /// <summary>
        /// Saves the Config to the given path
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            var undoCheckoutIfUnchanged = FileRequiresUndoCheckout(filePath);

            var serializer = new XmlSerializer(typeof(EarlyBoundGeneratorConfig));
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
            };
            using (var xmlWriter = XmlWriter.Create(filePath, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, this);
                xmlWriter.Close();
            }

            // Put pipe delimited values on new lines to make it easier to see changes in source control
            var xml = File.ReadAllText(filePath);
            xml = xml.Replace("|", "|" + Environment.NewLine);
            File.WriteAllText(filePath, xml);

            if (undoCheckoutIfUnchanged)
            {
                var tfs = new VsTfsSourceControlProvider();
                tfs.UndoCheckoutIfUnchanged(filePath);
            }
        }

        private bool FileRequiresUndoCheckout(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            var attributes = File.GetAttributes(filePath);

            var undoCheckoutIfUnchanged = false;
            if (!attributes.HasFlag(FileAttributes.ReadOnly))
            {
                return false;
            }

            attributes = attributes & ~FileAttributes.ReadOnly;
            if (ExtensionConfig.UseTfsToCheckoutFiles)
            {
                try
                {
                    var tfs = new VsTfsSourceControlProvider();
                    tfs.Checkout(filePath);
                    if (File.GetAttributes(filePath).HasFlag(FileAttributes.ReadOnly))
                    {
                        // something failed, just make it editable.
                        File.SetAttributes(filePath, attributes);
                    }
                    else
                    {
                        undoCheckoutIfUnchanged = true;
                    }
                }
                catch
                {
                    // eat it and just make it editable.
                    File.SetAttributes(filePath, attributes);
                }
            }
            else
            {
                File.SetAttributes(filePath, attributes);
            }

            return undoCheckoutIfUnchanged;
        }

        /// <summary>
        /// Returns the Setting Value
        /// </summary>
        /// <param name="creationType"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public string GetSettingValue(CreationType creationType, string setting)
        {
            var value = CommandLineArguments.FirstOrDefault(s => string.Equals(s.Name, setting, StringComparison.InvariantCultureIgnoreCase)
                                                                 && (s.SettingType == creationType || s.SettingType == CreationType.All));

            if (value == null)
            {
                throw new KeyNotFoundException("Unable to find setting for " + creationType + " " + setting);
            }

            return value.Value;
        }

        /// <summary>
        /// Returns the extension argument
        /// </summary>
        /// <param name="creationType"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public Argument GetExtensionArgument(CreationType creationType, CrmSrvUtilService service)
        {
            return GetExtensionArgument(creationType, service.ToString().ToLower());
        }

        /// <summary>
        /// Returns the extension argument
        /// </summary>
        /// <param name="creationType"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        public Argument GetExtensionArgument(CreationType creationType, string setting)
        {
            return ExtensionArguments.FirstOrDefault(a => a.SettingType == creationType &&
                                                          string.Equals(a.Name, setting, StringComparison.InvariantCultureIgnoreCase)) ??
                new Argument(creationType, setting, string.Empty);
        }

        /// <summary>
        /// Sets the extension argument
        /// </summary>
        /// <param name="creationType"></param>
        /// <param name="service"></param>
        /// <param name="value"></param>
        public void SetExtensionArgument(CreationType creationType, CrmSrvUtilService service, string value)
        {
            SetExtensionArgument(creationType, service.ToString().ToLower(), value);
        }

        /// <summary>
        /// Sets the extension arguments
        /// </summary>
        /// <param name="creationType"></param>
        /// <param name="setting"></param>
        /// <param name="value"></param>
        public void SetExtensionArgument(CreationType creationType, string setting, string value)
        {
            var argument = GetExtensionArgument(creationType, setting);

            if (argument == null)
            {
                if (value != null)
                {
                    ExtensionArguments.Add(new Argument { Name = setting, SettingType = creationType, Value = value });
                }
            }
            else if (value == null)
            {
                ExtensionArguments.Remove(argument);
            }
            else
            {
                argument.Value = value;
            }
        }

        private Argument GetUserArgument(CreationType creationType, string setting)
        {
            var argument = UserArguments.FirstOrDefault(s =>
                string.Equals(s.Name, setting, StringComparison.InvariantCultureIgnoreCase)
                && s.SettingType == creationType);

            return argument ?? new Argument(creationType, setting, string.Empty);
        }

        private void SetUserArgument(CreationType creationType, string setting, string value)
        {
            var argument = GetUserArgument(creationType, setting);

            if (argument == null)
            {
                if (value != null)
                {
                    UserArguments.Add(new Argument { Name = setting, SettingType = creationType, Value = value });
                }
            }
            else if (value == null)
            {
                UserArguments.Remove(argument);
            }
            else
            {
                argument.Value = value;
            }
        }

        internal struct UserArgumentNames
        {
            public const string GenerateActions = "generateActions";
            public const string Namespace = "namespace";
            public const string Out = "out";
            public const string ServiceContextName = "servicecontextname";
            public const string SuppressGeneratedCodeAttribute = "SuppressGeneratedCodeAttribute";
        }
    }
}

#pragma warning disable 1591
namespace DLaB.EarlyBoundGenerator.Settings.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class Config
    {
        public bool? AudibleCompletionNotification { get; set; }
        public string CrmSvcUtilRelativePath { get; set; }
        public bool? IncludeCommandLine { get; set; }
        public bool? MaskPassword { get; set; }
        public ExtensionConfig ExtensionConfig { get; set; }
        public List<Argument> ExtensionArguments { get; set; }
        public List<Argument> UserArguments { get; set; }
        public string Version { get; set; }
        public string WorkflowlessActions { get; set; }
    }
}
