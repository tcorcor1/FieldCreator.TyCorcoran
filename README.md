# FieldCreator.TyCorcoran
XRM Toolbox plugin used to bulk create Microsoft Dynamics fields via import of a csv.

How to Use:
# [1] Download the import template

Import template contains sample data for reference. For first-time users, I would recommend reviewing the Readme tab as it will outline important details such as what is required for import. Details also listed below:

| Field | Comments |
| --------------- | --------------- |
| Field Type | Required; Can only use types available in drop-down |
| Entity (schema name) | Required |
| Field Label | Required; please note character limit in tooltip |
| Field Schema Name | Required; please note character limit in tooltip |
| Required Level | Required; Can only use types available in drop-down |
| Solution Unique Name | Required |
| Description | Non-required; Please note character limit in tooltip |
| Audit Enabled | Non-required |
| Max Length (single line of text) | Non-required |
| Max Length (multiple lines of text) | Non-required |
| Max Value (Whole Number) | Non-required |
| Min Value (Whole Number) | Non-required |
| Option Set Type | Required for Field Type "Option Set" |
| Option Set Values | Required for Field Type "Option Set" |
| New Global Option Set Display Name | Required for New Global Option Set type |
| New Global Option Set Schema Name | Required for New Global Option Set type | 
| Existing Global Option Set Schema Name | Required for Existing Global Option Set type | 
| Precision | Non-required; Default is 2 | 
| Referenced Entity | Required for lookups; Must be schema name of entity | 
| One > N Relationship Schema Name | Required for lookups; Must be schema name of entity | 
 
Furnish the template with your data and save as a csv.

# [2] Upload CSV

Browse and select the location of your import template csv.

# [3] Submit and review results

Upon submission you will be provided updates as attributes are processed. All errors and successes will be logged and exportable once the job is complete. 

If you would like to resubmit, hit refresh and start again. 

