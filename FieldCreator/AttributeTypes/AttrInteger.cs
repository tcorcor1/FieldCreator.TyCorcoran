using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
	public class AttrInteger : AttrBase, IAttribute
	{
		public AttrInteger (Attribute attribute) : base(attribute)
		{
		}

		public AttributeMetadata ReturnAttributeMetadata (Attribute attribute)
		{
			try
			{
				var maxValue = (string.IsNullOrWhiteSpace(attribute.MaxValueNumber)) ? 2147483647 : Convert.ToInt32(attribute.MaxValueNumber);
				var minValue = (string.IsNullOrWhiteSpace(attribute.MinValueNumber)) ? -2147483648 : Convert.ToInt32(attribute.MinValueNumber);
				return new IntegerAttributeMetadata()
				{
					SchemaName = AttrSchemaName,
					DisplayName = new Label(AttrFieldLabel, CultureInfo.CurrentCulture.LCID),
					RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
					IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
					MaxValue = (maxValue <= minValue) ? 2147483647 : maxValue,
					MinValue = (minValue >= maxValue) ? -2147483648 : minValue,
					Description = (AttrDescription != null) ? new Label(AttrDescription, CultureInfo.CurrentCulture.LCID) : null
				};
			}
			catch (Exception ex)
			{
				throw new ArgumentException($"{attribute.FieldSchemaName}: {ex.Message}");
			}
		}
	}
}