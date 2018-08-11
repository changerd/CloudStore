namespace IBAstore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "DeliveryId", "dbo.Deliveries");
            DropIndex("dbo.Orders", new[] { "DeliveryId" });
            AddColumn("dbo.Orders", "City", c => c.String());
            AddColumn("dbo.Orders", "Street", c => c.String());
            AddColumn("dbo.Orders", "House", c => c.String());
            AddColumn("dbo.Orders", "Housing", c => c.String());
            AddColumn("dbo.Orders", "Flat", c => c.String());
            AddColumn("dbo.Orders", "Telephone", c => c.String());
            AddColumn("dbo.Orders", "TypeDeliveryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "TypeDeliveryId");
            AddForeignKey("dbo.Orders", "TypeDeliveryId", "dbo.TypeDeliveries", "Id", cascadeDelete: true);
            DropColumn("dbo.Orders", "DeliveryId");
            DropTable("dbo.Deliveries");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Deliveries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        City = c.String(),
                        Street = c.String(),
                        House = c.String(),
                        Housing = c.String(),
                        Flat = c.String(),
                        Telephone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "DeliveryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Orders", "TypeDeliveryId", "dbo.TypeDeliveries");
            DropIndex("dbo.Orders", new[] { "TypeDeliveryId" });
            DropColumn("dbo.Orders", "TypeDeliveryId");
            DropColumn("dbo.Orders", "Telephone");
            DropColumn("dbo.Orders", "Flat");
            DropColumn("dbo.Orders", "Housing");
            DropColumn("dbo.Orders", "House");
            DropColumn("dbo.Orders", "Street");
            DropColumn("dbo.Orders", "City");
            CreateIndex("dbo.Orders", "DeliveryId");
            AddForeignKey("dbo.Orders", "DeliveryId", "dbo.Deliveries", "Id", cascadeDelete: true);
        }
    }
}
