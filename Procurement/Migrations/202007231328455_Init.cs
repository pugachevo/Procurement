namespace Procurement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Sum = c.Decimal(nullable: false, storeType: "money"),
                        Date = c.DateTime(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Goods",
                c => new
                    {
                        GoodID = c.Int(nullable: false, identity: true),
                        Сost = c.Decimal(nullable: false, storeType: "money"),
                        Quantity = c.Int(nullable: false),
                        OrdersID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GoodID)
                .ForeignKey("dbo.Orders", t => t.OrdersID, cascadeDelete: true)
                .Index(t => t.OrdersID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        LastName = c.String(maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        StartDate = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateStoredProcedure(
                "dbo.Users_Insert",
                p => new
                    {
                        LastName = p.String(maxLength: 50),
                        Name = p.String(maxLength: 50),
                        StartDate = p.DateTime(),
                    },
                body:
                    @"INSERT [dbo].[Users]([LastName], [Name], [StartDate])
                      VALUES (@LastName, @Name, @StartDate)
                      
                      DECLARE @UserID int
                      SELECT @UserID = [UserID]
                      FROM [dbo].[Users]
                      WHERE @@ROWCOUNT > 0 AND [UserID] = scope_identity()
                      
                      SELECT t0.[UserID], t0.[RowVersion]
                      FROM [dbo].[Users] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[UserID] = @UserID"
            );
            
            CreateStoredProcedure(
                "dbo.Users_Update",
                p => new
                    {
                        UserID = p.Int(),
                        LastName = p.String(maxLength: 50),
                        Name = p.String(maxLength: 50),
                        StartDate = p.DateTime(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"UPDATE [dbo].[Users]
                      SET [LastName] = @LastName, [Name] = @Name, [StartDate] = @StartDate
                      WHERE (([UserID] = @UserID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Users] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[UserID] = @UserID"
            );
            
            CreateStoredProcedure(
                "dbo.Users_Delete",
                p => new
                    {
                        UserID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"DELETE [dbo].[Users]
                      WHERE (([UserID] = @UserID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.Users_Delete");
            DropStoredProcedure("dbo.Users_Update");
            DropStoredProcedure("dbo.Users_Insert");
            DropForeignKey("dbo.Orders", "UserID", "dbo.Users");
            DropForeignKey("dbo.Goods", "OrdersID", "dbo.Orders");
            DropIndex("dbo.Goods", new[] { "OrdersID" });
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropTable("dbo.Users");
            DropTable("dbo.Goods");
            DropTable("dbo.Orders");
        }
    }
}
