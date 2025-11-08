// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable

namespace Core2D.ViewModels.Wizard.Export.Steps;

public enum SummaryIssueSeverity
{
    Warning,
    Error
}

public sealed class SummaryIssueViewModel
{
    public SummaryIssueViewModel(SummaryIssueSeverity severity, string message)
    {
        Severity = severity;
        Message = message;
    }

    public SummaryIssueSeverity Severity { get; }

    public string Message { get; }

    public string Icon => Severity == SummaryIssueSeverity.Error ? "✖" : "⚠";

    public bool IsError => Severity == SummaryIssueSeverity.Error;

    public bool IsWarning => Severity == SummaryIssueSeverity.Warning;
}
