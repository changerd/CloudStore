namespace IBAstore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration4 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Telephone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Telephone", c => c.String());
        }
    }
}
