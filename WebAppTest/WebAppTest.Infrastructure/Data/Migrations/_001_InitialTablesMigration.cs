using FluentMigrator;

namespace WebAppTest.WebApi.Data.Migrations;

[Migration(20240101000000)]
public class _1_InitialTablesMigration : Migration
{
    public override void Up()
    {
        Create.Table("Departments")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(255).NotNullable()
            .WithColumn("Phone").AsString(50).Nullable();

        Create.Table("Employees")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(255).NotNullable()
            .WithColumn("Surname").AsString(255).NotNullable()
            .WithColumn("Phone").AsString(50).Nullable()
            .WithColumn("CompanyId").AsInt32().NotNullable()
            .WithColumn("DepartmentId").AsInt32().NotNullable();

        Create.Table("Passports")
            .WithColumn("Id").AsInt32().PrimaryKey()
            .WithColumn("Type").AsString(50).NotNullable()
            .WithColumn("Number").AsString(100).NotNullable();
        
        Create.ForeignKey("FK_Employees_Departments")
            .FromTable("Employees").ForeignColumn("DepartmentId")
            .ToTable("Departments").PrimaryColumn("Id");

        Create.ForeignKey("FK_Passports_Employees")
            .FromTable("Passports").ForeignColumn("Id")
            .ToTable("Employees").PrimaryColumn("Id")
            .OnDelete(System.Data.Rule.Cascade);

        Create.UniqueConstraint("UC_Passports_Id")
            .OnTable("Passports")
            .Column("Id");

        Create.Index("IX_Employees_CompanyId")
            .OnTable("Employees")
            .OnColumn("CompanyId");

        Create.Index("IX_Employees_DepartmentId")
            .OnTable("Employees")
            .OnColumn("DepartmentId");
    }

    public override void Down()
    {
        Delete.Table("Passports");
        Delete.Table("Employees");
        Delete.Table("Departments");
    }
}