using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonIssueRepro
{
    public class ColumnMetadata
    {
        public Guid Id { get; set; }

        ICollection<LanguageTitle> _Titles = new HashSet<LanguageTitle>();
        public ICollection<LanguageTitle> LanguageTitles
        {
            get => _Titles;
            set => _Titles = value.ToHashSet();
        }

        public ColumnMetadataEntityType EntityType { get; set; }

        public string? ColumnName { get; set; }

        public Guid? GroupId { get; set; }

        ICollection<string> _ExceptionRoleNames = new HashSet<string>();
        public ICollection<string> ExceptionRoleNames
        {
            get => _ExceptionRoleNames;
            set => _ExceptionRoleNames = value.ToHashSet();
        }

        public bool IsHidden { get; set; }

        public bool IsDeleted { get; set; }

        public int? SortOrder { get; set; }
    }

    public enum ColumnMetadataEntityType
    {
        Contact
    }

    public partial class LanguageTitle
    {
        public string LanguageId { get; set; } = "eng";

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}