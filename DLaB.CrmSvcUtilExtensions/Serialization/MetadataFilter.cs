﻿using System;
using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Serialization
{

    [DataContract(Namespace = "http://DLaB.CrmSvcUtilExtensions")]
    public class MetadataFilter
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public int PrimaryObjectTypeCode { get; set; }
        [DataMember]
        public int SecondaryObjectTypeCode { get; set; }
        [DataMember]
        public bool IsVisible { get; set; }

        public MetadataFilter(SdkMessageFilter filter)
        {
            Id = filter.Id;
            PrimaryObjectTypeCode = filter.PrimaryObjectTypeCode;
            SecondaryObjectTypeCode = filter.SecondaryObjectTypeCode;
            IsVisible = filter.IsVisible;
        }

        public static implicit operator SdkMessageFilter(MetadataFilter filter)
        {
            return new SdkMessageFilter(filter.Id, filter.PrimaryObjectTypeCode, filter.SecondaryObjectTypeCode, filter.IsVisible);
        }
    }
}
