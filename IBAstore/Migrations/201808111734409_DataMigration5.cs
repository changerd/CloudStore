namespace IBAstore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SaleStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SaleStats", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SaleStats", "ProductId", "dbo.Products");
            DropIndex("dbo.SaleStats", new[] { "ProductId" });
            DropIndex("dbo.SaleStats", new[] { "UserId" });
            DropTable("dbo.SaleStats");
        }
    }
}
