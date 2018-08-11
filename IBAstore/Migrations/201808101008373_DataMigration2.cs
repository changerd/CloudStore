namespace IBAstore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Description");
        }
    }
}
