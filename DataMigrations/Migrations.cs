using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Localization;

namespace Belitsoft.Orchard.Survey.DataMigrations
{
    public class Migrations : DataMigrationImpl
    {

        public Localizer T { get; set; }

        public Migrations()
        {
            T = NullLocalizer.Instance;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable("SurveyWidgetPartRecord", table => table
                                                                             .ContentPartRecord()
                                                                             .Column<int>("SurveyId"));

            SchemaBuilder.CreateTable("SurveyPartRecord", table => table
                                                                       .ContentPartRecord()
                                                                       .Column<bool>("IsNotShowResultAfterAnswer"));

            SchemaBuilder.CreateTable("SurveyListWidgetPartRecord", table => table
                                                                                 .ContentPartRecord()
                                                                                 .Column<int>("ViewCount")
                                                                                 .Column<bool>("Paging"));


            ContentDefinitionManager.AlterPartDefinition("SurveyPart", builder => builder
                                                                                      .WithField("DueDate",
                                                                                                 fieldBuilder =>
                                                                                                 fieldBuilder.OfType(
                                                                                                     "DateTimeField")
                                                                                                             .WithSetting
                                                                                                     ("DateTimeFieldSettings.Required",
                                                                                                      "True")));

            ContentDefinitionManager.AlterTypeDefinition("Survey",
                                                         cfg =>
                                                         cfg.WithPart("CommonPart",
                                                                      p =>
                                                                      p.WithSetting(
                                                                          "OwnerEditorSettings.ShowOwnerEditor", "False"))
                                                            .WithPart("BodyPart",
                                                                      bcfg =>
                                                                      bcfg.WithSetting(
                                                                          "BodyPartSettings.FlavorDefault", "text"))
                                                            .WithPart("SurveyPart")
                                                            .Creatable()
                                                            .Draftable());

            SchemaBuilder.CreateTable("AnswerRecord",
                                      table => table.Column<int>("Id", cfg => cfg.PrimaryKey().Identity())
                                                    .Column<bool>("Value")
                                                    .Column<string>("Text", cfg => cfg.WithLength(1024))
                                                    .Column<int>("SurveyId"));

            SchemaBuilder.CreateTable("UserAnswerRecord", table => table
                                                                       .Column<int>("Id",
                                                                                    cfg => cfg.PrimaryKey().Identity())
                                                                       .Column<int>("AnswerId")
                                                                       .Column<int>("SurveyId")
                                                                       .Column<int>("UserId"));

            ContentDefinitionManager.AlterTypeDefinition("SurveyListWidget", cfg => cfg
                                                                                        .WithPart("CommonPart",
                                                                                                  p =>
                                                                                                  p.WithSetting(
                                                                                                      "DateEditorSettings.ShowDateEditor",
                                                                                                      "False")
                                                                                                   .WithSetting(
                                                                                                       "OwnerEditorSettings.ShowOwnerEditor",
                                                                                                       "False"))
                                                                                        .WithPart("WidgetPart")
                                                                                        .WithPart("SurveyListWidget")
                                                                                        .WithSetting("Stereotype",
                                                                                                     "Widget"));

            ContentDefinitionManager.AlterTypeDefinition("SurveyWidget", cfg => cfg

                                                                                    .WithPart("CommonPart",
                                                                                              p =>
                                                                                              p.WithSetting(
                                                                                                  "DateEditorSettings.ShowDateEditor",
                                                                                                  "False")
                                                                                               .WithSetting(
                                                                                                   "OwnerEditorSettings.ShowOwnerEditor",
                                                                                                   "False"))
                                                                                    .WithPart("WidgetPart")
                                                                                    .WithPart("SurveyWidget")
                                                                                    .WithSetting("Stereotype", "Widget"));
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("AnswerRecord", table => table
                                                                  .AddColumn<int>("StartValue",
                                                                                  cfg => cfg.WithDefault(0)));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable("SurveyPartRecord", table => table
                                                                      .AddColumn<string>("Title"));

            return 3;

        }

        public int UpdateFrom3()
        {
            ContentDefinitionManager.AlterTypeDefinition("Survey",
                                                         cfg =>
                                                         cfg.WithPart("TitlePart",
                                                                      p =>
                                                                      p.WithSetting(
                                                                          "OwnerEditorSettings.ShowOwnerEditor", "False")));

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.DropTable("SurveyListWidgetPartRecord");
            ContentDefinitionManager.DeleteTypeDefinition("SurveyListWidget");
            return 5;
        }

        public int UpdateFrom5()
        {
            ContentDefinitionManager.AlterTypeDefinition("SurveyPart", cfg => cfg.WithSetting("clonable", "False"));
            return 6;
        }
    }
}