using Microsoft.CommonDataService;
using Microsoft.CommonDataService.Modeling;

[RelationalEntitySet("Microsoft.CommonDataService.CommonEntitySets", "FabrikamDrivers_", Version = "1.0")]
[RelationalIndex("Idx1", true, new[] { "PrimaryId" }, Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
public class Driver : TypedRelationalEntity
{
    public Driver() { }

    [RelationalField("PrimaryId", IsMandatory = false)]
    public string PrimaryId { get; set; }

    [RelationalField("RouteTitle", IsMandatory = false)]
    public string RouteTitle { get; set; }

    [RelationalField("AverageMiles", IsMandatory = false)]
    public decimal AverageMiles { get; set; }
}

[RelationalEntitySet("Microsoft.CommonDataService.CommonEntitySets", "FabrikamEmployee_", Version = "1.0")]
[RelationalIndex("Idx1", true, new[] { "EmployeeID" }, Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
public class Employee : TypedRelationalEntity
{
    public Employee() { }

    [RelationalField("EmployeeID", IsMandatory = true)]
    public string EmployeeID { get; set; }

    [RelationalField("FullName", IsMandatory = true)]
    public string FullName { get; set; }

    [RelationalField(name: "FabrikamEmployee_Driver")]
    [RelationalFieldRelation(sourceCardinality: RelationCardinality.ZeroOne, pairedPropertyNamespace: "Microsoft.CommonDataService.CommonEntitySets", pairedPropertyName: "FabrikamEmployee_Driver", sourceDeleteAction: RelationalDeleteAction.None)]
    public IList<RelationalEntityReference<RideShare>> FabrikamEmployee_Driver { get; set; }

}

[RelationalEntitySet("Microsoft.CommonDataService.CommonEntitySets", "FabrikamRideShare_", Version = "1.0")]
[RelationalIndex("Idx1", true, new[] { "PrimaryId" }, Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
public class RideShare : TypedRelationalEntity
{
    public RideShare() { }

    [RelationalField("PrimaryId", IsMandatory = false)]
    public string PrimaryId { get; set; }

    [RelationalField("ExpectedDistance", IsMandatory = false)]
    public Int64 ExpectedDistance { get; set; }

    [RelationalField("ActualDistance", IsMandatory = false)]
    public Int64 ActualDistance { get; set; }

    [RelationalField("RideCompleted", IsMandatory = false)]
    public bool RideCompleted { get; set; }

    [RelationalField("HomeToWorkStart", IsMandatory = false)]
    public UtcDateTime HomeToWorkStart { get; set; }

    [RelationalField("HomeToWorkEnd", IsMandatory = false)]
    public UtcDateTime HomeToWorkEnd { get; set; }

    [RelationalField("WorkToHomeStart", IsMandatory = false)]
    public UtcDateTime WorkToHomeStart { get; set; }

    [RelationalField("WorkToHomeEnd", IsMandatory = false)]
    public UtcDateTime WorkToHomeEnd { get; set; }

    //FabrikamEmployee_Driver
    [RelationalField(name: "FabrikamEmployee_Driver", Namespace = "Microsoft.CommonDataService.CommonEntitySets")]
    [RelationalFieldRelation(sourceCardinality: RelationCardinality.ZeroMany, pairedPropertyNamespace: "Microsoft.CommonDataService.CommonEntitySets", pairedPropertyName: "FabrikamEmployee_Driver")]
    public RelationalEntityReference<Employee> FabrikamEmployee_Driver { get; set; }



    //StartLocation
    //EndLocation
    //FabrikamEmployee_Passenger1
    //FabrikamEmployee_Passenger2
    //FabrikamEmployee_Passenger3
    //FabrikamEmployee_Passenger4
    //FabrikamEmployee_Passenger5
    //FabrikamEmployee_Passenger6
}