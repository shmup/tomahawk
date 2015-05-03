namespace Tomahawk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class textlength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Messages", "Text", c => c.String(maxLength: 140));
            AlterColumn("dbo.Replies", "Text", c => c.String(maxLength: 140));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Replies", "Text", c => c.String());
            AlterColumn("dbo.Messages", "Text", c => c.String());
        }
    }
}
