namespace IBAstore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            DropForeignKey("dbo.ProductRequests", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProductRequests", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductRequests", new[] { "ProductId" });
            DropIndex("dbo.ProductRequests", new[] { "UserId" });
            DropTable("dbo.ProductRequests");
        }
    }
}
