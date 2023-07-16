namespace EnfusionDatabaseFramework.Drivers.WebProxy.Core.Conditions;

public class DbFindFieldCondition : DbFindCondition
{
    public required string FieldPath { get; set; }

    private DbFindFieldConditionPath? _parsed; // Parse only once and then cache the result

    public DbFindFieldConditionPath AsParsed => _parsed ??= new(FieldPath);
}

public class DbFindFieldConditionPath
{
    public List<DbFindFieldConditionPathSegment> Segments { get; set; } = new();

    public DbFindFieldConditionPath()
    {
    }

    public DbFindFieldConditionPath(string fieldPath)
    {
        foreach (string pathSplit in fieldPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var modifierSplits = pathSplit.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            if (modifierSplits.Count == 0)
                continue;

            string fieldName = modifierSplits.ElementAt(0);
            DbFindFieldConditionPathModifier modifiers = 0;

            void CompleteSegment()
            {
                var segment = new DbFindFieldConditionPathSegment(fieldName, modifiers);
                fieldName = string.Empty;
                modifiers = 0;

                if (segment.CollectionIndices?.Count > 0 || segment.CollectionTypeFilters?.Count > 0)
                {
                    if ((segment.Modifiers & (DbFindFieldConditionPathModifier.KEYS | DbFindFieldConditionPathModifier.VALUES)) != 0)
                    {
                        var nextModifiers = segment.Modifiers;
                        segment.Modifiers = DbFindFieldConditionPathModifier.ANY;
                        Segments.Add(segment);
                        segment = new DbFindFieldConditionPathSegment(modifiers: nextModifiers);
                    }
                    else if (segment.Modifiers == 0)
                    {
                        segment.Modifiers = DbFindFieldConditionPathModifier.ANY;
                    }
                    else if (!segment.Modifiers.HasFlag(DbFindFieldConditionPathModifier.ALL))
                    {
                        segment.Modifiers |= DbFindFieldConditionPathModifier.ANY;
                    }
                }

                Segments.Add(segment);
            }

            foreach (var modifierSplit in modifierSplits.Skip(1))
            {
                switch (modifierSplit)
                {
                    case "any":
                    {
                        if (modifiers.HasFlag(DbFindFieldConditionPathModifier.ANY) || modifiers.HasFlag(DbFindFieldConditionPathModifier.ALL))
                            CompleteSegment();

                        modifiers |= DbFindFieldConditionPathModifier.ANY;
                        break;
                    }

                    case "all":
                    {
                        if (modifiers.HasFlag(DbFindFieldConditionPathModifier.ANY) || modifiers.HasFlag(DbFindFieldConditionPathModifier.ALL))
                            CompleteSegment();

                        modifiers |= DbFindFieldConditionPathModifier.ALL;
                        break;
                    }

                    case "keys":
                    {
                        if (modifiers != 0)
                            CompleteSegment();

                        modifiers |= DbFindFieldConditionPathModifier.KEYS;
                        break;
                    }

                    case "values":
                    {
                        if (modifiers != 0)
                            CompleteSegment();

                        modifiers |= DbFindFieldConditionPathModifier.VALUES;
                        break;
                    }

                    case "count":
                    {
                        modifiers |= DbFindFieldConditionPathModifier.COUNT;
                        break;
                    }

                    case "length":
                    {
                        modifiers |= DbFindFieldConditionPathModifier.LENGTH;
                        break;
                    }

                    default:
                    {
                        throw new ArgumentException($"Unknown modifier {modifierSplit} in field path.");
                    }
                }
            }

            CompleteSegment();
        }
    }
}

public class DbFindFieldConditionPathSegment
{
    public string? FieldName { get; set; }

    public HashSet<int>? CollectionIndices { get; set; }

    public HashSet<Typename>? CollectionTypeFilters { get; set; }

    public DbFindFieldConditionPathModifier Modifiers { get; set; }

    public DbFindFieldConditionPathSegment(string? fieldName = default, DbFindFieldConditionPathModifier modifiers = default)
    {
        if (!string.IsNullOrWhiteSpace(fieldName))
        {
            bool typenameMode = false, intMode = false;

            if (fieldName.StartsWith("{"))
            {
                foreach (var arraySplit in fieldName[1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (!typenameMode && !intMode)
                    {
                        if (int.TryParse(arraySplit, out _))
                        {
                            intMode = true;
                            CollectionIndices = new();
                        }
                        else
                        {
                            typenameMode = true;
                            CollectionTypeFilters = new();
                        }
                    }

                    if (typenameMode)
                    {
                        CollectionTypeFilters!.Add(arraySplit);
                    }
                    else
                    {
                        CollectionIndices!.Add(int.Parse(arraySplit));
                    }
                }
            }
            else
            {
                FieldName = fieldName;
            }
        }

        Modifiers = modifiers;
    }
}

[Flags]
public enum DbFindFieldConditionPathModifier
{
    ANY = 1,
    ALL = 2,
    KEYS = 4,
    VALUES = 8,
    COUNT = 16,
    LENGTH = 32
}
