using Microsoft.CodeAnalysis.Classification;

namespace CsharpToHtml;

public readonly record struct Tag(int Position, string? ClassName)
{
    public static void AddTo(IEnumerable<ClassifiedSpan> classifiedSpans, IList<Tag> output)
    {
        foreach (var span in classifiedSpans)
        {
            var @class = ClassTable.TypeToClass(span.ClassificationType);

            if (@class is null) continue;

            output.Add(new(span.TextSpan.Start, @class));
            output.Add(new(span.TextSpan.End, null));
        }
    }

    public static Builder GetBuilder() => new();

    public struct Builder
    {
        private readonly List<Tag> _tags = new();
        public Builder() { }

        public Builder Append(ClassifiedSpan span)
        {
            var @class = ClassTable.TypeToClass(span.ClassificationType);

            if (@class is null) return this;

            _tags.Add(new(span.TextSpan.Start, @class));
            _tags.Add(new(span.TextSpan.End, null));

            return this;
        }

        public Builder Append(IEnumerable<ClassifiedSpan> classifiedSpans)
        {
            foreach (var span in classifiedSpans)
                Append(span);

            return this;
        }

        public Queue Build()
        {
            _tags.Sort((x, y) => x.Position.CompareTo(y.Position));
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
