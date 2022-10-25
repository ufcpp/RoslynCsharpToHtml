using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;

namespace CsharpToHtml;

public readonly record struct Tag(int Position, int OppositePosition, string? ClassName, string? DiagnosticId) : IComparable<Tag>
{
    public static Tag Open(TextSpan span, string className, string? diagnosticId = null) => new(span.Start, span.End, className, diagnosticId);
    public static Tag Close(TextSpan span) => new(span.End, span.Start, null, null);

    public static Builder GetBuilder() => new();

    public bool IsClose => ClassName is null;

    public int CompareTo(Tag other)
    {
        var pos = Position.CompareTo(other.Position);
        if (pos != 0) return pos;

        // close tags must preceed open tags.
        if (IsClose && !other.IsClose) return -1;
        if (!IsClose && other.IsClose) return 1;

        // reverse order for opposite tags.
        return -OppositePosition.CompareTo(other.OppositePosition);
    }

    public struct Builder
    {
        private readonly List<Tag> _tags = new();
        public Builder() { }

        public Builder Append(ClassifiedSpan span)
        {
            var @class = ClassTable.TypeToClass(span.ClassificationType);

            if (@class is null) return this;

            _tags.Add(Open(span.TextSpan, @class));
            _tags.Add(Close(span.TextSpan));

            return this;
        }

        public Builder Append(IEnumerable<ClassifiedSpan> classifiedSpans)
        {
            foreach (var span in classifiedSpans)
                Append(span);

            return this;
        }

        public Builder Append(Diagnostic diag)
        {
            var sevirity = diag.Severity switch
            {
                DiagnosticSeverity.Warning => "warning",
                DiagnosticSeverity.Error => "error",
                _ => null,
            };
            if (sevirity is null) return this;

            _tags.Add(Open(diag.Location.SourceSpan, sevirity, diag.Id));
            _tags.Add(Close(diag.Location.SourceSpan));

            return this;
        }

        public Builder Append(IEnumerable<Diagnostic> diags)
        {
            foreach (var diag in diags)
                Append(diag);

            return this;
        }

        public Queue Build()
        {
            _tags.Sort();
            return new(_tags);
        }
    }

    public struct Queue
    {
        private Queue<Tag> _queue;
        internal Queue(IEnumerable<Tag> items) => _queue = new(items);

        public bool TryGet(int position, out Tag tag)
        {
            if (_queue.Count == 0)
            {
                tag = default;
                return false;
            }

            tag = _queue.Peek();
            if (tag.Position == position)
            {
                _queue.Dequeue();
                return true;
            }
            return false;
        }
    }
}
