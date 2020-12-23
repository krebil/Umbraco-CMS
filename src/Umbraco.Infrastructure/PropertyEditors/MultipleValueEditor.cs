﻿using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Serialization;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;

namespace Umbraco.Web.PropertyEditors
{
    /// <summary>
    /// A value editor to handle posted json array data and to return array data for the multiple selected csv items
    /// </summary>
    /// <remarks>
    /// This is re-used by editors such as the multiple drop down list or check box list
    /// </remarks>
    public class MultipleValueEditor : DataValueEditor
    {
        private readonly ILogger<MultipleValueEditor> _logger;

        public MultipleValueEditor(
            ILogger<MultipleValueEditor> logger,
            IDataTypeService dataTypeService,
            ILocalizationService localizationService,
            ILocalizedTextService localizedTextService,
            IShortStringHelper shortStringHelper,
            IJsonSerializer jsonSerializer,
            DataEditorAttribute attribute
            )
            : base(dataTypeService, localizationService, localizedTextService, shortStringHelper, jsonSerializer, attribute)
        {
            _logger = logger;
        }

        /// <summary>
        /// Override so that we can return an array to the editor for multi-select values
        /// </summary>
        /// <param name="property"></param>
        /// <param name="dataTypeService"></param>
        /// <param name="culture"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        public override object ToEditor(IProperty property, string culture = null, string segment = null)
        {
            var json = base.ToEditor(property, culture, segment).ToString();
            return JsonConvert.DeserializeObject<string[]>(json) ?? Array.Empty<string>();
        }

        /// <summary>
        /// When multiple values are selected a json array will be posted back so we need to format for storage in
        /// the database which is a comma separated string value
        /// </summary>
        /// <param name="editorValue"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public override object FromEditor(Core.Models.Editors.ContentPropertyData editorValue, object currentValue)
        {
            var json = editorValue.Value as JArray;
            if (json == null)
            {
                return null;
            }

            var values = json.Select(item => item.Value<string>()).ToArray();

            return JsonConvert.SerializeObject(values);
        }
    }
}