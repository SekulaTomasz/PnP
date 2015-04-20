﻿using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml.V201505;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml
{
    internal class XMLPnPSchemaV201505Formatter :
        IXMLSchemaFormatter, ITemplateFormatter
    {
        private TemplateProviderBase _provider;

        public void Initialize(TemplateProviderBase provider)
        {
            this._provider = provider;
        }

        string IXMLSchemaFormatter.NamespaceUri
        {
            get { return (XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05); }
        }

        string IXMLSchemaFormatter.NamespacePrefix
        {
            get { return (XMLConstants.PROVISIONING_SCHEMA_PREFIX); }
        }

        public bool IsValid(Stream template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            // Load the template into an XDocument
            XDocument xml = XDocument.Load(template);

            // Load the XSD embedded resource
            Stream stream = typeof(XMLPnPSchemaV201505Formatter)
                .Assembly
                .GetManifestResourceStream("OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml.ProvisioningSchema-2015-05.xsd");

            // Prepare the XML Schema Set
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05,
                new XmlTextReader(stream));

            Boolean result = true;
            xml.Validate(schemas, (o, e) =>
            {
                result = false;
            });

            return (result);
        }

        Stream ITemplateFormatter.ToFormattedTemplate(ProvisioningTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            V201505.SharePointProvisioningTemplate result = new V201505.SharePointProvisioningTemplate();

            V201505.Provisioning wrappedResult = new V201505.Provisioning();
            wrappedResult.Preferences = new V201505.Preferences
            {
                Generator = this.GetType().Assembly.FullName
            };
            wrappedResult.Templates = new V201505.Templates[] { 
                new V201505.Templates 
                { 
                    ID = String.Format("CONTAINER-{0}", template.ID),
                    SharePointProvisioningTemplate = new V201505.SharePointProvisioningTemplate[]
                    {
                        result
                    }
                }
            };

            // Translate basic properties
            result.ID = template.ID;
            result.Version = (Decimal)template.Version;
            result.VersionSpecified = true;
            result.SitePolicy = template.SitePolicy;

            // Translate PropertyBagEntries, if any
            if (template.PropertyBagEntries != null && template.PropertyBagEntries.Count > 0)
            {
                result.PropertyBagEntries =
                    (from bag in template.PropertyBagEntries
                     select new V201505.StringDictionaryItem
                     {
                         Key = bag.Key,
                         Value = bag.Value,
                     }).ToArray();
            }
            else
            {
                result.PropertyBagEntries = null;
            }

            // Translate Security configuration, if any
            if (template.Security != null)
            {
                result.Security = new V201505.SharePointProvisioningTemplateSecurity();

                if (template.Security.AdditionalAdministrators != null && template.Security.AdditionalAdministrators.Count > 0)
                {
                    result.Security.AdditionalAdministrators =
                        (from user in template.Security.AdditionalAdministrators
                         select new V201505.User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                else
                {
                    result.Security.AdditionalAdministrators = null;
                }

                if (template.Security.AdditionalOwners != null && template.Security.AdditionalOwners.Count > 0)
                {
                    result.Security.AdditionalOwners =
                        (from user in template.Security.AdditionalOwners
                         select new V201505.User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                else
                {
                    result.Security.AdditionalOwners = null;
                }

                if (template.Security.AdditionalMembers != null && template.Security.AdditionalMembers.Count > 0)
                {
                    result.Security.AdditionalMembers =
                        (from user in template.Security.AdditionalMembers
                         select new V201505.User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                else
                {
                    result.Security.AdditionalMembers = null;
                }

                if (template.Security.AdditionalVisitors != null && template.Security.AdditionalVisitors.Count > 0)
                {
                    result.Security.AdditionalVisitors =
                        (from user in template.Security.AdditionalVisitors
                         select new V201505.User
                         {
                             Name = user.Name,
                         }).ToArray();
                }
                else
                {
                    result.Security.AdditionalVisitors = null;
                }
            }

            // Translate Site Columns (Fields), if any
            if (template.SiteFields != null && template.SiteFields.Count > 0)
            {
                result.SiteFields = new V201505.SharePointProvisioningTemplateSiteFields
                {
                    Any =
                        (from field in template.SiteFields
                         select field.SchemaXml.ToXmlElement()).ToArray(),
                };
            }
            else
            {
                result.SiteFields = null;
            }

            // Translate ContentTypes, if any
            if (template.ContentTypes != null && template.ContentTypes.Count > 0)
            {
                result.ContentTypes = (from ct in template.ContentTypes
                                       select new V201505.ContentType
            {
                ID = ct.ID,
                Description = ct.Description,
                Group = ct.Group,
                Name = ct.Name,
                FieldRefs = ct.FieldRefs.Count > 0 ?
                    (from fieldRef in ct.FieldRefs
                     select new V201505.FieldRef
                     {
                         ID = fieldRef.ID.ToString(),
                         Hidden = fieldRef.Hidden,
                         Required = fieldRef.Required
                     }).ToArray() : null,
            }).ToArray();

            }
            else
            {
                result.ContentTypes = null;
            }

            // Translate Lists Instances, if any
            if (template.Lists != null && template.Lists.Count > 0)
            {
                result.Lists =
                    (from list in template.Lists
                     select new V201505.ListInstance
                     {
                         ContentTypesEnabled = list.ContentTypesEnabled,
                         Description = list.Description,
                         DocumentTemplate = list.DocumentTemplate,
                         EnableVersioning = list.EnableVersioning,
                         Hidden = list.Hidden,
                         MinorVersionLimit = list.MinorVersionLimit,
                         MinorVersionLimitSpecified = true,
                         MaxVersionLimit = list.MaxVersionLimit,
                         MaxVersionLimitSpecified = true,
                         OnQuickLaunch = list.OnQuickLaunch,
                         RemoveExistingContentTypes = list.RemoveExistingContentTypes,
                         TemplateFeatureID = list.TemplateFeatureID != Guid.Empty ? list.TemplateFeatureID.ToString() : null,
                         TemplateType = list.TemplateType,
                         Title = list.Title,
                         Url = list.Url,
                         ContentTypeBindings = list.ContentTypeBindings.Count > 0 ?
                            (from contentTypeBinding in list.ContentTypeBindings
                             select new V201505.ContentTypeBinding
                             {
                                 ContentTypeID = contentTypeBinding.ContentTypeID,
                                 Default = contentTypeBinding.Default,
                             }).ToArray() : null,
                         Views = list.Views.Count > 0 ?
                         new V201505.ListInstanceViews
                         {
                             Any =
                                (from view in list.Views
                                 select view.SchemaXml.ToXmlElement()).ToArray(),
                             RemoveExistingViews = list.RemoveExistingViews,
                         } : null,
                         Fields = list.Fields.Count > 0 ?
                         new V201505.ListInstanceFields
                         {
                             Any =
                             (from field in list.Fields
                              select field.SchemaXml.ToXmlElement()).ToArray(),
                         } : null,
                         FieldRefs = list.FieldRefs.Count > 0 ?
                         (from fieldRef in list.FieldRefs
                          select new V201505.FieldRef
                          {
                              ID = fieldRef.ID.ToString(),
                          }).ToArray() : null,
                         DataRows = list.DataRows.Count > 0 ?
                             new List<DataValue[]>(
                                from row in list.DataRows
                                select new List<DataValue>(
                                    from value in row.Values
                                    select new DataValue { FieldName = value.Key, Value = value.Value }
                                    ).ToArray()
                                ).ToArray() : null,
                     }).ToArray();
            }
            else
            {
                result.Lists = null;
            }

            // Translate Features, if any
            if (template.Features != null)
            {
                result.Features = new V201505.SharePointProvisioningTemplateFeatures();

                // TODO: This nullability check could be useless, because
                // the SiteFeatures property is initialized in the Features
                // constructor
                if (template.Features.SiteFeatures != null && template.Features.SiteFeatures.Count > 0)
                {
                    result.Features.SiteFeatures =
                        (from feature in template.Features.SiteFeatures
                         select new V201505.Feature
                         {
                             ID = feature.ID.ToString(),
                             Deactivate = feature.Deactivate,
                         }).ToArray();
                }
                else
                {
                    result.Features.SiteFeatures = null;
                }

                // TODO: This nullability check could be useless, because
                // the WebFeatures property is initialized in the Features
                // constructor
                if (template.Features.WebFeatures != null && template.Features.WebFeatures.Count > 0)
                {
                    result.Features.WebFeatures =
                        (from feature in template.Features.WebFeatures
                         select new V201505.Feature
                         {
                             ID = feature.ID.ToString(),
                             Deactivate = feature.Deactivate,
                         }).ToArray();
                }
                else
                {
                    result.Features.WebFeatures = null;
                }
            }

            // Translate CustomActions, if any
            if (template.CustomActions != null)
            {
                result.CustomActions = new V201505.SharePointProvisioningTemplateCustomActions();

                if (template.CustomActions.SiteCustomActions != null && template.CustomActions.SiteCustomActions.Count > 0)
                {
                    result.CustomActions.SiteCustomActions =
                        (from customAction in template.CustomActions.SiteCustomActions
                         select new V201505.CustomAction
                         {
                             Description = customAction.Description,
                             Enabled = customAction.Enabled,
                             Group = customAction.Group,
                             ImageUrl = customAction.ImageUrl,
                             Location = customAction.Location,
                             Name = customAction.Name,
                             Rights = customAction.RightsValue,
                             RightsSpecified = true,
                             ScriptBlock = customAction.ScriptBlock,
                             ScriptSrc = customAction.ScriptSrc,
                             Sequence = customAction.Sequence,
                             SequenceSpecified = true,
                             Title = customAction.Title,
                             Url = customAction.Url,
                         }).ToArray();
                }
                else
                {
                    result.CustomActions.SiteCustomActions = null;
                }

                if (template.CustomActions.WebCustomActions != null && template.CustomActions.WebCustomActions.Count > 0)
                {
                    result.CustomActions.WebCustomActions =
                        (from customAction in template.CustomActions.WebCustomActions
                         select new V201505.CustomAction
                         {
                             Description = customAction.Description,
                             Enabled = customAction.Enabled,
                             Group = customAction.Group,
                             ImageUrl = customAction.ImageUrl,
                             Location = customAction.Location,
                             Name = customAction.Name,
                             Rights = customAction.RightsValue,
                             RightsSpecified = true,
                             ScriptBlock = customAction.ScriptBlock,
                             ScriptSrc = customAction.ScriptSrc,
                             Sequence = customAction.Sequence,
                             SequenceSpecified = true,
                             Title = customAction.Title,
                             Url = customAction.Url,
                         }).ToArray();
                }
                else
                {
                    result.CustomActions.WebCustomActions = null;
                }
            }

            // Translate Files, if any
            if (template.Files != null && template.Files.Count > 0)
            {
                result.Files =
                    (from file in template.Files
                     select new V201505.File
                     {
                         Overwrite = file.Overwrite,
                         Src = file.Src,
                         Folder = file.Folder,
                         WebParts = file.WebParts.Count > 0 ?
                            (from wp in file.WebParts
                             select new V201505.WebPartPageWebPart
                             {
                                 Zone = wp.Zone,
                                 Order = (int)wp.Order,
                                 Contents = wp.Contents,
                                 Title = wp.Title,
                             }).ToArray() : null,
                     }).ToArray();
            }
            else
            {
                result.Files = null;
            }

            // Translate Pages, if any
            if (template.Pages != null && template.Pages.Count > 0)
            {
                var pages = new List<V201505.Page>();

                foreach (var page in template.Pages)
                {
                    var schemaPage = new V201505.Page();

                    var pageLayout = WIKIPAGELAYOUT.OneColumn;
                    switch (page.Layout)
                    {
                        case WikiPageLayout.OneColumn:
                            pageLayout = WIKIPAGELAYOUT.OneColumn;
                            break;
                        case WikiPageLayout.OneColumnSideBar:
                            pageLayout = WIKIPAGELAYOUT.OneColumnSidebar;
                            break;
                        case WikiPageLayout.TwoColumns:
                            pageLayout = WIKIPAGELAYOUT.TwoColumns;
                            break;
                        case WikiPageLayout.TwoColumnsHeader:
                            pageLayout = WIKIPAGELAYOUT.TwoColumnsHeader;
                            break;
                        case WikiPageLayout.TwoColumnsHeaderFooter:
                            pageLayout = WIKIPAGELAYOUT.TwoColumnsHeaderFooter;
                            break;
                        case WikiPageLayout.ThreeColumns:
                            pageLayout = WIKIPAGELAYOUT.ThreeColumns;
                            break;
                        case WikiPageLayout.ThreeColumnsHeader:
                            pageLayout = WIKIPAGELAYOUT.ThreeColumnsHeader;
                            break;
                        case WikiPageLayout.ThreeColumnsHeaderFooter:
                            pageLayout = WIKIPAGELAYOUT.ThreeColumnsHeaderFooter;
                            break;
                    }
                    schemaPage.Layout = pageLayout;
                    schemaPage.Overwrite = page.Overwrite;

                    schemaPage.WebParts = page.WebParts.Count > 0 ?
                        (from wp in page.WebParts
                         select new V201505.WikiPageWebPart
                         {
                             Column = (int)wp.Column,
                             Row = (int)wp.Row,
                             Contents = wp.Contents,
                             Title = wp.Title,
                         }).ToArray() : null;

                    schemaPage.Url = page.Url;

                    pages.Add(schemaPage);
                }

                result.Pages = pages.ToArray();
            }

            // Translate Taxonomy elements, if any
            if (template.TermGroups != null && template.TermGroups.Count > 0)
            {
                result.TermGroups =
                    (from grp in template.TermGroups
                     select new V201505.TermGroup
                     {
                         Name = grp.Name,
                         ID = grp.ID.ToString(),
                         Description = grp.Description,
                         TermSets = (
                            from termSet in grp.TermSets
                            select new V201505.TermSet
                            {
                                ID = termSet.ID.ToString(),
                                Name = termSet.Name,
                                Description = termSet.Description,
                                Language = termSet.Language.HasValue ? termSet.Language.Value : 0,
                                LanguageSpecified = termSet.Language.HasValue,
                                Terms = termSet.Terms.FromModelTermsToSchemaTerms(),
                            }).ToArray(),
                     }).ToArray();
            }

            // Translate ComposedLook, if any
            if (template.ComposedLook != null)
            {
                result.ComposedLook = new V201505.ComposedLook
                {
                    AlternateCSS = template.ComposedLook.AlternateCSS,
                    BackgroundFile = template.ComposedLook.BackgroundFile,
                    ColorFile = template.ComposedLook.ColorFile,
                    FontFile = template.ComposedLook.FontFile,
                    MasterPage = template.ComposedLook.MasterPage,
                    Name = template.ComposedLook.Name,
                    SiteLogo = template.ComposedLook.SiteLogo,
                    Version = template.ComposedLook.Version,
                    VersionSpecified = true,
                };
            }

            // Translate Providers, if any
            if (template.Providers != null && template.Providers.Count > 0)
            {
                result.Providers =
                    (from provider in template.Providers
                     select new V201505.Provider
                     {
                         HandlerType = String.Format("{0}, {1}", provider.Type, provider.Assembly),
                         Configuration = provider.Configuration != null ? provider.Configuration.ToXmlNode() : null,
                         Enabled = provider.Enabled,
                     }).ToArray();
            }
            else
            {
                result.Providers = null;
            }

            XmlSerializerNamespaces ns =
                new XmlSerializerNamespaces();
            ns.Add(((IXMLSchemaFormatter)this).NamespacePrefix,
                ((IXMLSchemaFormatter)this).NamespaceUri);

            var output = XMLSerializer.SerializeToStream<V201505.Provisioning>(wrappedResult, ns);
            output.Position = 0;
            return (output);
        }

        public ProvisioningTemplate ToProvisioningTemplate(Stream template)
        {
            return (this.ToProvisioningTemplate(template, null));
        }

        public ProvisioningTemplate ToProvisioningTemplate(Stream template, String identifier)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            // Crate a copy of the source stream
            MemoryStream sourceStream = new MemoryStream();
            template.CopyTo(sourceStream);
            sourceStream.Position = 0;

            // Check the provided template against the XML schema
            if (!this.IsValid(sourceStream))
            {
                // TODO: Use resource file
                throw new ApplicationException("The provided template is not valid!");
            }

            sourceStream.Position = 0;
            XDocument xml = XDocument.Load(sourceStream);
            XNamespace pnp = XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05;

            // Prepare a variable to hold the single source formatted template
            V201505.SharePointProvisioningTemplate source = null;

            // Prepare a variable to hold the resulting ProvisioningTemplate instance
            ProvisioningTemplate result = new ProvisioningTemplate();

            // Determine if we're working on a wrapped SharePointProvisioningTemplate or not
            if (xml.Root.Name == pnp + "Provisioning")
            {
                // Deserialize the whole wrapper
                V201505.Provisioning wrappedResult = XMLSerializer.Deserialize<V201505.Provisioning>(xml);

                // Handle the wrapper schema parameters
                if (wrappedResult.Preferences != null && 
                    wrappedResult.Preferences.Parameters != null &&
                    wrappedResult.Preferences.Parameters.Length > 0)
                {
                    foreach (var parameter in wrappedResult.Preferences.Parameters)
                    {
                        result.Parameters.Add(parameter.Key, parameter.Text != null ? parameter.Text.Aggregate(String.Empty, (acc, i) => acc + i) : null);
                    }
                }

                foreach (var templates in wrappedResult.Templates)
                {
                    // Let's see if we have an in-place template with the provided ID or if we don't have a provided ID at all
                    source = templates.SharePointProvisioningTemplate.FirstOrDefault(spt => spt.ID == identifier || String.IsNullOrEmpty(identifier));

                    // If we don't have a template, but there are external file references
                    if (source == null && templates.SharePointProvisioningTemplateFile.Length > 0)
                    {
                        // Otherwise let's see if we have an external file for the template
                        var externalSource = templates.SharePointProvisioningTemplateFile.FirstOrDefault(sptf => sptf.ID == identifier);

                        Stream externalFileStream = this._provider.Connector.GetFileStream(externalSource.File);
                        xml = XDocument.Load(externalFileStream);

                        if (xml.Root.Name != pnp + "SharePointProvisioningTemplate")
                        {
                            throw new ApplicationException("Invalid external file format. Expected a SharePointProvisioningTemplate file!");
                        }
                        else
                        {
                            source = XMLSerializer.Deserialize<V201505.SharePointProvisioningTemplate>(xml);
                        }
                    }

                    if (source != null) {
                        break; 
                    }
                }
            }
            else if (xml.Root.Name == pnp + "SharePointProvisioningTemplate")
            {
                var IdAttribute = xml.Root.Attribute("ID");

                // If there is a provided ID, and if it doesn't equal the current ID
                if (!String.IsNullOrEmpty(identifier) &&
                    IdAttribute != null && 
                    IdAttribute.Value != identifier)
                {
                    // TODO: Use resource file
                    throw new ApplicationException("The provided template identifier is not available!");
                }
                else
                {
                    source = XMLSerializer.Deserialize<V201505.SharePointProvisioningTemplate>(xml);
                }
            }

            // Translate basic properties
            result.ID = source.ID;
            result.Version = (Double)source.Version;
            result.SitePolicy = source.SitePolicy;

            // Translate PropertyBagEntries, if any
            if (source.PropertyBagEntries != null)
            {
                result.PropertyBagEntries.AddRange(
                    from bag in source.PropertyBagEntries
                    select new Model.PropertyBagEntry
                    {
                        Key = bag.Key,
                        Value = bag.Value,
                    });
            }

            // Translate Security configuration, if any
            if (source.Security != null)
            {
                if (source.Security.AdditionalAdministrators != null)
                {
                    result.Security.AdditionalAdministrators.AddRange(
                    from user in source.Security.AdditionalAdministrators
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
                if (source.Security.AdditionalOwners != null)
                {
                    result.Security.AdditionalOwners.AddRange(
                    from user in source.Security.AdditionalOwners
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
                if (source.Security.AdditionalMembers != null)
                {
                    result.Security.AdditionalMembers.AddRange(
                    from user in source.Security.AdditionalMembers
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
                if (source.Security.AdditionalVisitors != null)
                {
                    result.Security.AdditionalVisitors.AddRange(
                    from user in source.Security.AdditionalVisitors
                    select new Model.User
                    {
                        Name = user.Name,
                    });
                }
            }

            // Translate Site Columns (Fields), if any
            if ((source.SiteFields != null) && (source.SiteFields.Any != null))
            {
                result.SiteFields.AddRange(
                    from field in source.SiteFields.Any
                    select new Field
                    {
                        SchemaXml = field.OuterXml,
                    });
            }

            // Translate ContentTypes, if any
            if ((source.ContentTypes != null) && (source.ContentTypes != null))
            {
                result.ContentTypes.AddRange(
                    from contentType in source.ContentTypes
                    select new ContentType(
                        contentType.ID,
                        contentType.Name,
                        contentType.Description,
                        contentType.Group,
                        contentType.Sealed,
                        contentType.Hidden,
                        contentType.ReadOnly,
                        (contentType.DocumentTemplate != null ?
                            contentType.DocumentTemplate.TargetName : null),
                        contentType.Overwrite,
                        (contentType.FieldRefs != null ?
                            (from fieldRef in contentType.FieldRefs
                             select new Model.FieldRef
                             {
                                 ID = Guid.Parse(fieldRef.ID),
                                 Hidden = fieldRef.Hidden,
                                 Required = fieldRef.Required
                             }) : null)
                        )
                    );
            }

            // Translate Lists Instances, if any
            if (source.Lists != null)
            {
                result.Lists.AddRange(
                    from list in source.Lists
                    select new Model.ListInstance(
                        (list.ContentTypeBindings != null ?
                                (from contentTypeBinding in list.ContentTypeBindings
                                 select new Model.ContentTypeBinding
                                 {
                                     ContentTypeID = contentTypeBinding.ContentTypeID,
                                     Default = contentTypeBinding.Default,
                                 }) : null),
                        (list.Views != null ?
                                (from view in list.Views.Any
                                 select new View
                                 {
                                     SchemaXml = view.OuterXml,
                                 }) : null),
                        (list.Fields != null ?
                                (from field in list.Fields.Any
                                 select new Field
                                 {
                                     SchemaXml = field.OuterXml,
                                 }) : null),
                        (list.FieldRefs != null ?
                                 (from fieldRef in list.FieldRefs
                                  select new Model.FieldRef
                                  {
                                      ID = Guid.Parse(fieldRef.ID)
                                  }) : null),
                        (list.DataRows != null ?
                                 (from dataRow in list.DataRows
                                  select new Model.DataRow(
                                     (from dataValue in dataRow
                                      select dataValue).ToDictionary(k => k.FieldName, v => v.Value)
                                  )).ToList() : null)
                        )
                    {
                        ContentTypesEnabled = list.ContentTypesEnabled,
                        Description = list.Description,
                        DocumentTemplate = list.DocumentTemplate,
                        EnableVersioning = list.EnableVersioning,
                        Hidden = list.Hidden,
                        MinorVersionLimit = list.MinorVersionLimitSpecified ? list.MinorVersionLimit : 0,
                        MaxVersionLimit = list.MaxVersionLimitSpecified ? list.MaxVersionLimit : 0,
                        OnQuickLaunch = list.OnQuickLaunch,
                        RemoveExistingContentTypes = list.RemoveExistingContentTypes,
                        TemplateFeatureID = !String.IsNullOrEmpty(list.TemplateFeatureID) ? Guid.Parse(list.TemplateFeatureID) : Guid.Empty,
                        RemoveExistingViews = list.Views != null ? list.Views.RemoveExistingViews : false,
                        TemplateType = list.TemplateType,
                        Title = list.Title,
                        Url = list.Url,
                    });
            }

            // Translate Features, if any
            if (source.Features != null)
            {
                if (result.Features.SiteFeatures != null && source.Features.SiteFeatures != null)
                {
                    result.Features.SiteFeatures.AddRange(
                        from feature in source.Features.SiteFeatures
                        select new Model.Feature
                        {
                            ID = new Guid(feature.ID),
                            Deactivate = feature.Deactivate,
                        });
                }
                if (result.Features.WebFeatures != null && source.Features.WebFeatures != null)
                {
                    result.Features.WebFeatures.AddRange(
                        from feature in source.Features.WebFeatures
                        select new Model.Feature
                        {
                            ID = new Guid(feature.ID),
                            Deactivate = feature.Deactivate,
                        });
                }
            }

            // Translate CustomActions, if any
            if (source.CustomActions != null)
            {
                if (result.CustomActions.SiteCustomActions != null && source.CustomActions.SiteCustomActions != null)
                {
                    result.CustomActions.SiteCustomActions.AddRange(
                        from customAction in source.CustomActions.SiteCustomActions
                        select new Model.CustomAction
                        {
                            Description = customAction.Description,
                            Enabled = customAction.Enabled,
                            Group = customAction.Group,
                            ImageUrl = customAction.ImageUrl,
                            Location = customAction.Location,
                            Name = customAction.Name,
                            RightsValue = customAction.RightsSpecified ? customAction.Rights : 0,
                            ScriptBlock = customAction.ScriptBlock,
                            ScriptSrc = customAction.ScriptSrc,
                            Sequence = customAction.SequenceSpecified ? customAction.Sequence : 100,
                            Title = customAction.Title,
                            Url = customAction.Url,
                        });
                }
                if (result.CustomActions.WebCustomActions != null && source.CustomActions.WebCustomActions != null)
                {
                    result.CustomActions.WebCustomActions.AddRange(
                        from customAction in source.CustomActions.WebCustomActions
                        select new Model.CustomAction
                        {
                            Description = customAction.Description,
                            Enabled = customAction.Enabled,
                            Group = customAction.Group,
                            ImageUrl = customAction.ImageUrl,
                            Location = customAction.Location,
                            Name = customAction.Name,
                            RightsValue = customAction.RightsSpecified ? customAction.Rights : 0,
                            ScriptBlock = customAction.ScriptBlock,
                            ScriptSrc = customAction.ScriptSrc,
                            Sequence = customAction.SequenceSpecified ? customAction.Sequence : 100,
                            Title = customAction.Title,
                            Url = customAction.Url,
                        });
                }
            }

            // Translate Files, if any
            if (source.Files != null)
            {
                result.Files.AddRange(
                    from file in source.Files
                    select new Model.File(file.Src,
                        file.Folder,
                        file.Overwrite,
                        file.Create,
                        file.WebParts != null ?
                            (from wp in file.WebParts
                             select new Model.WebPart
                                 {
                                     Order = (uint)wp.Order,
                                     Zone = wp.Zone,
                                     Title = wp.Title,
                                     Contents = wp.Contents
                                 }) : null
                            )
                    );
            }

            // Translate Pages, if any
            if (source.Pages != null)
            {
                foreach (var page in source.Pages)
                {

                    var pageLayout = WikiPageLayout.OneColumn;
                    switch (page.Layout)
                    {
                        case WIKIPAGELAYOUT.OneColumn:
                            pageLayout = WikiPageLayout.OneColumn;
                            break;
                        case WIKIPAGELAYOUT.OneColumnSidebar:
                            pageLayout = WikiPageLayout.OneColumnSideBar;
                            break;
                        case WIKIPAGELAYOUT.TwoColumns:
                            pageLayout = WikiPageLayout.TwoColumns;
                            break;
                        case WIKIPAGELAYOUT.TwoColumnsHeader:
                            pageLayout = WikiPageLayout.TwoColumnsHeader;
                            break;
                        case WIKIPAGELAYOUT.TwoColumnsHeaderFooter:
                            pageLayout = WikiPageLayout.TwoColumnsHeaderFooter;
                            break;
                        case WIKIPAGELAYOUT.ThreeColumns:
                            pageLayout = WikiPageLayout.ThreeColumns;
                            break;
                        case WIKIPAGELAYOUT.ThreeColumnsHeader:
                            pageLayout = WikiPageLayout.ThreeColumnsHeader;
                            break;
                        case WIKIPAGELAYOUT.ThreeColumnsHeaderFooter:
                            pageLayout = WikiPageLayout.ThreeColumnsHeaderFooter;
                            break;
                    }

                    result.Pages.Add(new Model.Page(page.Url, page.Overwrite, pageLayout,
                        (page.WebParts != null ?
                            (from wp in page.WebParts
                             select new Model.WebPart
                             {
                                 Title = wp.Title,
                                 Column = (uint)wp.Column,
                                 Row = (uint)wp.Row,
                                 Contents = wp.Contents

                             }).ToList() : null)));

                }
            }

            if (source.TermGroups != null)
            {
                result.TermGroups.AddRange(
                    from termGroup in source.TermGroups
                    select new Model.TermGroup(
                        Guid.Parse(termGroup.ID),
                        termGroup.Name,
                        new List<Model.TermSet>(
                            from termSet in termGroup.TermSets
                            select new Model.TermSet(
                                Guid.Parse(termSet.ID),
                                termSet.Name,
                                termSet.LanguageSpecified ? (int?)termSet.Language : null,
                                termSet.Terms.FromSchemaTermsToModelTerms())
                            {
                                Description = termSet.Description,
                            })
                        )
                        {
                            Description = termGroup.Description,
                        });
            }

            // Translate ComposedLook, if any
            if (source.ComposedLook != null)
            {
                result.ComposedLook.AlternateCSS = source.ComposedLook.AlternateCSS;
                result.ComposedLook.BackgroundFile = source.ComposedLook.BackgroundFile;
                result.ComposedLook.ColorFile = source.ComposedLook.ColorFile;
                result.ComposedLook.FontFile = source.ComposedLook.FontFile;
                result.ComposedLook.MasterPage = source.ComposedLook.MasterPage;
                result.ComposedLook.Name = source.ComposedLook.Name;
                result.ComposedLook.SiteLogo = source.ComposedLook.SiteLogo;
                result.ComposedLook.Version = source.ComposedLook.Version;
            }

            // Translate Providers, if any
            if (source.Providers != null)
            {
                foreach (var provider in source.Providers)
                {
                    if (!String.IsNullOrEmpty(provider.HandlerType))
                    {
                        var handlerType = Type.GetType(provider.HandlerType, false);
                        if (handlerType != null)
                        {
                            result.Providers.Add(
                                new Model.Provider
                                {
                                    Assembly = handlerType.AssemblyQualifiedName,
                                    Type = handlerType.FullName,
                                    Configuration = provider.Configuration != null ? provider.Configuration.ToProviderConfiguration() : null,
                                    Enabled = provider.Enabled,
                                });
                        }
                    }
                }
            }

            return (result);
        }
    }

    internal static class TaxonomyTermExtensions
    {
        public static V201505.Term[] FromModelTermsToSchemaTerms(this List<Model.Term> terms)
        {
            V201505.Term[] result = (
                from term in terms
                select new V201505.Term
                {
                    ID = term.ID.ToString(),
                    Name = term.Name,
                    Description = term.Description,
                    Owner = term.Owner,
                    IsAvailableForTagging = term.IsAvailableForTagging.HasValue ? term.IsAvailableForTagging.Value : false,
                    IsAvailableForTaggingSpecified = term.IsAvailableForTagging.HasValue,
                    CustomSortOrder = term.CustomSortOrder,
                    ChildTerms = new TermChildTerms { Items = term.Terms.FromModelTermsToSchemaTerms() },
                    CustomProperties = term.Properties.Count > 0 ?
                        (from p in term.Properties
                         select new V201505.StringDictionaryItem
                         {
                             Key = p.Key,
                             Value = p.Value
                         }).ToArray() : null,
                    LocalCustomProperties = term.LocalProperties.Count > 0 ?
                        (from p in term.LocalProperties
                         select new V201505.StringDictionaryItem
                         {
                             Key = p.Key,
                             Value = p.Value
                         }).ToArray() : null,
                    Labels = term.Labels.Count > 0 ?
                        (from l in term.Labels
                         select new V201505.TermLabelsLabel
                         {
                             Language = l.Language,
                             IsDefaultForLanguage = l.IsDefaultForLanguage.HasValue ? l.IsDefaultForLanguage.Value : false,
                             IsDefaultForLanguageSpecified = l.IsDefaultForLanguage.HasValue,
                             Value = l.Value,
                         }).ToArray() : null,
                }).ToArray();

            return (result);
        }

        public static List<Model.Term> FromSchemaTermsToModelTerms(this V201505.Term[] terms)
        {
            List<Model.Term> result = new List<Model.Term>(
                from term in terms
                select new Model.Term(
                    Guid.Parse(term.ID),
                    term.Name,
                    null, // TODO: language
                    (term.ChildTerms != null && term.ChildTerms.Items != null) ? term.ChildTerms.Items.FromSchemaTermsToModelTerms() : null,
                    term.Labels != null ? 
                    (new List<Model.TermLabel>(
                        from label in term.Labels
                        select new Model.TermLabel
                        {
                            Language = label.Language,
                            Value = label.Value,
                            IsDefaultForLanguage = label.IsDefaultForLanguageSpecified ? label.IsDefaultForLanguage : false,
                        }
                    )) : null,
                    term.CustomProperties != null ? term.CustomProperties.ToDictionary(k => k.Key, v => v.Value) : null,
                    term.LocalCustomProperties != null ? term.LocalCustomProperties.ToDictionary(k => k.Key, v => v.Value) : null
                    )
                    {
                        CustomSortOrder = term.CustomSortOrder,
                        IsAvailableForTagging = term.IsAvailableForTaggingSpecified ? term.IsAvailableForTagging : false,
                        Owner = term.Owner,
                    }
                );

            return (result);
        }
    }
}
