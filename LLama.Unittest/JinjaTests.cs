using LLamaSharp.Jinja;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLama.Unittest.Jinja;

public sealed class JinjaUnitTest
{
    private static readonly Options lstrip_blocks = new()
    {
        TrimBlocks = false,
        LStripBlocks = true,
        KeepTrailingNewline = false,
    };

    private static readonly Options trim_blocks = new()
    {
        TrimBlocks = true,
        LStripBlocks = false,
        KeepTrailingNewline = false,
    };

    private static readonly Options lstrip_trim_blocks = new()
    {
        TrimBlocks = true,
        LStripBlocks = true,
        KeepTrailingNewline = false,
    };

    private static string Render(string templateStr, object? bindings, Options? options = null)
    {
        var root = Parser.Parse(templateStr, options ?? Options.Default);
        var context = Context.Make(bindings);
        return root.Render(context);
    }

    private class TestBinding1
    {
        [JsonPropertyName("z")]
        public int[][] Z { get; } = [[1, 10], [2, 20]];
    }

    [Fact]
    public void SyntaxTests()
    {
        Assert.Equal("a", Render("{{ ' a '.strip() }}", null, null));
        Assert.Equal("a ", Render("{{ ' a '.lstrip() }}", null, null));
        Assert.Equal(" a", Render("{{ ' a '.rstrip() }}", null, null));
        Assert.Equal("bcXYZab", Render("{{ 'abcXYZabc'.strip('ac') }}", null, null));

        Assert.Equal(@"[""a"", ""b""]", Render("{{ 'a b'.split(' ') | tojson }}", null, null));

        Assert.Equal(
            "Ok",
Render("{{ 'ok'.capitalize() }}", null, null));
        Assert.Equal("aouiXYZaouiXYZaoui",
Render("{{ 'abcXYZabcXYZabc'.replace('bc', 'oui') }}", null, null));
        Assert.Equal("okXYZokXYZabc",
Render("{{ 'abcXYZabcXYZabc'.replace('abc', 'ok', 2) }}", null, null));
        Assert.Equal("abcXYZabcXYZabc",
Render("{{ 'abcXYZabcXYZabc'.replace('def', 'ok') }}", null, null));

        Assert.Equal("HELLO WORLD", Render("{{ 'hello world'.upper() }}", null, null));
        Assert.Equal("MIXED", Render("{{ 'MiXeD'.upper() }}", null, null));
        Assert.Equal("", Render("{{ ''.upper() }}", null, null));

        Assert.Equal("hello world", Render("{{ 'HELLO WORLD'.lower() }}", null, null));
        Assert.Equal("mixed", Render("{{ 'MiXeD'.lower() }}", null, null));
        Assert.Equal("", Render("{{ ''.lower() }}", null, null));

        Assert.Equal(
            "ok",
Render("{# Hey\nHo #}{#- Multiline...\nComments! -#}{{ 'ok' }}{# yo #}", null, null));

        Assert.Equal(
            "    b",
Render("  {% set _ = 1 %}    {% set _ = 2 %}b", null, lstrip_trim_blocks));
        Assert.Equal(
            "        1",
Render("{%- if True %}        {% set _ = x %}{%- endif %}{{ 1 }}", null, lstrip_trim_blocks));

        Assert.Equal("\n", Render("    {% if True %}\n    {% endif %}", null, lstrip_blocks));
        Assert.Equal("", Render("    {% if True %}\n    {% endif %}", null, lstrip_trim_blocks));
        Assert.Equal("        ", Render("    {% if True %}\n    {% endif %}", null, trim_blocks));

        Assert.Equal("      ", Render("  {% set _ = 1 %}    ", null, null));
        Assert.Equal("    ", Render("  {% set _ = 1 %}    ", null, lstrip_blocks));
        Assert.Equal("      ", Render("  {% set _ = 1 %}    ", null, trim_blocks));
        Assert.Equal("    ", Render("  {% set _ = 1 %}    ", null, lstrip_trim_blocks));

        Assert.Equal("  \n            \n                ", Render("  \n    {% set _ = 1 %}        \n                ", null, null));
        Assert.Equal("  \n        \n                ", Render("  \n    {% set _ = 1 %}        \n                ", null, lstrip_blocks));
        Assert.Equal("  \n            \n                ", Render("  \n    {% set _ = 1 %}        \n                ", null, trim_blocks));
        Assert.Equal("  \n        \n                ", Render("  \n    {% set _ = 1 %}        \n                ", null, lstrip_trim_blocks));

        Assert.Equal("\n  ", Render("{% set _ = 1 %}\n  ", null, null));
        Assert.Equal("\n  ", Render("{% set _ = 1 %}\n  ", null, lstrip_blocks));
        Assert.Equal("  ", Render("{% set _ = 1 %}\n  ", null, trim_blocks));
        Assert.Equal("  ", Render("{% set _ = 1 %}\n  ", null, lstrip_trim_blocks));

        Assert.Equal(
            "[2, 3]",
Render("{{ range(*[2,4]) | list }}", null, null));
        Assert.Equal(
            "1, 0, 10, -10, 10, -10, 0, 0, 2, 0, 0, ",
Render("{% for i in [true, false, 10, -10, 10.1, -10.1, None, 'a', '2', {}, [1]] %}{{ i | int }}, {% endfor %}", null, null));
        Assert.Equal(
            "abc",
Render("{% filter trim %} abc {% endfilter %}", null, null));
        Assert.Equal(
            "[1, 2, 3]",
Render("{{ [1] + [2, 3] }}", null, null));
        Assert.Equal(
            "abc",
Render("{{ 'AbC' | lower }}", null, null));
        Assert.Equal(
            "ME",
Render("{{ 'me' | upper }}", null, null));
        Assert.Equal(
            "the default1",
Render("{{ foo | default('the default') }}{{ 1 | default('nope') }}", null, null));
        Assert.Equal(
            "the default1",
Render("{{ '' | default('the default', true) }}{{ 1 | default('nope', true) }}", null, null));
        Assert.Equal(
            "a\n  b\n|  a\n  b\n",
Render("{% set txt = 'a\\nb\\n' %}{{ txt | indent(2) }}|{{ txt | indent(2, first=true) }}", null, null));
        Assert.Equal(
            "        1",
Render("{%- if True %}        {% set _ = x %}{%- endif %}{{ 1 }}", null, lstrip_trim_blocks));
        Assert.Equal(
            "a  b",
Render("  {{- 'a' -}}{{ '  ' }}{{- 'b' -}}  ", null, null));
        Assert.Equal(
            "bc",
Render(@"{{ ""abcd""[1:-1] }}", null, null));
        Assert.Equal(
            "[1, 2]",
Render("{{ [0, 1, 2, 3][1:-1] }}", null, null));
        Assert.Equal(
            "9",
Render(@"{{ ""123456789"" | length }}", null, null));
        Assert.Equal(
            "        end",
Render("    {%- if True %}{%- endif %}{{ '        ' }}{%- for x in [] %}foo{% endfor %}end", null, null));
        Assert.Equal(
            "False",
Render("{% set ns = namespace(is_first=false, nottool=false, and_or=true, delme='') %}{{ ns.is_first }}", null, null));
        Assert.Equal(
            "True,False",
Render("{{ {} is mapping }},{{ '' is mapping }}", null, null));
        Assert.Equal(
            "True,True",
Render("{{ {} is iterable }},{{ '' is iterable }}", null, null));
        Assert.Equal(
            "a,b,",
Render(@"{% for x in [""a"", ""b""] %}{{ x }},{% endfor %}", null, null));
        Assert.Equal(
            "a,b,",
Render(@"{% for x in {""a"": 1, ""b"": 2} %}{{ x }},{% endfor %}", null, null));
        Assert.Equal(
            "a,b,",
Render(@"{% for x in ""ab"" %}{{ x }},{% endfor %}", null, null));
        Assert.Equal(
            "Foo Bar",
Render("{{ 'foo bar'.title() }}", null, null));
        Assert.Equal(
            "1",
Render("{{ 1 | safe }}", null, null));
        Assert.Equal(
            "True,False",
Render("{{ 'abc'.startswith('ab') }},{{ ''.startswith('a') }}", null, null));
        Assert.Equal(
            "True,False",
Render("{{ 'abc'.endswith('bc') }},{{ ''.endswith('a') }}", null, null));
        Assert.Equal(
            "[]",
Render(@"{{ none | selectattr(""foo"", ""equalto"", ""bar"") | list }}", null, null));
        Assert.Equal(
            "True,False",
Render(@"{{ 'a' in {""a"": 1} }},{{ 'a' in {} }}", null, null));
        Assert.Equal(
            "True,False",
Render(@"{{ 'a' in [""a""] }},{{ 'a' in [] }}", null, null));
        Assert.Equal("True,False",
Render("{{ 'a' in 'abc' }},{{ 'd' in 'abc' }}", null, null));
        Assert.Equal("False,True",
Render("{{ 'a' not in 'abc' }},{{ 'd' not in 'abc' }}", null, null));
        Assert.Equal("['a', 'a']",
Render("{{ ['a', 'b', 'c', 'a'] | select('in', ['a']) | list }}", null, null));
        Assert.Equal("['a', 'b'],[]",
Render("{{ {'a': 1, 'b': 2}.keys() | list }},{{ {}.keys() | list }}", null, null));
        Assert.Equal(
            "[{'a': 1}]",
Render(@"{{ [{""a"": 1}, {""a"": 2}, {}] | selectattr(""a"", ""equalto"", 1) | list }}", null, null));
        Assert.Equal(
            "[{'a': 2}, {}]",
Render(@"{{ [{""a"": 1}, {""a"": 2}, {}] | rejectattr(""a"", ""equalto"", 1) | list }}", null, null));
        Assert.Equal(
            "[1, 2]",
Render(@"{{ [{""a"": 1}, {""a"": 2}] | map(attribute=""a"") | list }}", null, null));
        Assert.Equal(
            "[0, 1]",
Render(@"{{ ["""", ""a""] | map(""length"") | list }}", null, null));
        Assert.Equal(
            "2",
Render("{{ range(3) | last }}", null, null));
        Assert.Equal(
            "True",
Render("{% set foo = true %}{{ foo is defined }}", null, null));
        Assert.Equal(
            "False",
Render("{% set foo = true %}{{ not foo is defined }}", null, null));
        Assert.Equal(
            "True",
Render("{% set foo = true %}{{ foo is true }}", null, null));
        Assert.Equal(
            "False",
Render("{% set foo = true %}{{ foo is false }}", null, null));
        Assert.Equal(
            "True",
Render("{% set foo = false %}{{ foo is not true }}", null, null));
        Assert.Equal(
            "False",
Render("{% set foo = false %}{{ foo is not false }}", null, null));
        Assert.Equal(
            @"{""a"": ""b""}",
Render(@"{{ {""a"": ""b""} | tojson }}", null, null));
        Assert.Equal(
            "{'a': 'b'}",
Render(@"{{ {""a"": ""b""} }}", null, null));

        string trim_tmpl =
            "\n" +
            "  {% if true %}Hello{% endif %}  \n" +
            "...\n" +
            "\n";
        Assert.Equal(
                    "\n  Hello  \n...\n",
    Render(trim_tmpl, null, trim_blocks));
        Assert.Equal(
            "\n  Hello  \n...\n",
Render(trim_tmpl, null, null));
        Assert.Equal(
            "\nHello  \n...\n",
Render(trim_tmpl, null, lstrip_blocks));
        Assert.Equal(
            "\nHello  \n...\n",
Render(trim_tmpl, null, lstrip_trim_blocks));
        Assert.Equal(
            "a | b | c",
Render(@"{%- set separator = joiner(' | ') -%}
            {%- for item in [""a"", ""b"", ""c""] %}{{ separator() }}{{ item }}{% endfor -%}", null, null));
        Assert.Equal(
            "a\nb",
Render("a\nb\n", null, null));
        Assert.Equal(
            " a\n",
Render("  {{- ' a\n'}}", null, trim_blocks));
        Assert.Equal(
            "but first, mojitos!1,2,3",
Render(@"
            {%- for x in range(3) -%}
                {%- if loop.first -%}
                    but first, mojitos!
                {%- endif -%}
                {{ loop.index }}{{ "","" if not loop.last -}}
            {%- endfor -%}
        ", null, null));
        Assert.Equal(
            "a0b",
Render("{{ 'a' + [] | length | string + 'b' }}", null, null));
        Assert.Equal(
            "1, 2, 3...",
Render("{{ [1, 2, 3] | join(', ') + '...' }}", null, null));
        Assert.Equal(
            "Tools: 1, 3...",
Render("{{ 'Tools: ' + [1, 2, 3] | reject('equalto', 2) | join(', ') + '...' }}", null, null));
        Assert.Equal(
            "Tools: 2...",
Render("{{ 'Tools: ' + [1, 2, 3] | select('equalto', 2) | join(', ') + '...' }}", null, null));
        Assert.Equal(
            "1, 2, 3",
Render("{{ [1, 2, 3] | join(', ') }}", null, null));
        Assert.Equal(
            "0,1,2,",
Render("{% for i in range(3) %}{{i}},{% endfor %}", null, null));
        Assert.Equal(
            "1Hello there2",
Render("{% set foo %}Hello {{ 'there' }}{% endset %}{{ 1 ~ foo ~ 2 }}", null, null));
        Assert.Equal(
            "[1, False, 2, '3']",
Render("{{ [1, False, 2, '3', 1, '3', False] | unique | list }}", null, null));
        Assert.Equal(
            "1",
Render("{{ range(5) | length % 2 }}", null, null));
        Assert.Equal(
            "True,False",
Render("{{ range(5) | length % 2 == 1 }},{{ [] | length > 0 }}", null, null));
        Assert.Equal(
            "False",
Render(
"{{ messages[0]['role'] != 'system' }}",
new { messages = new[] { new { role = "system" } } },
null
        ));
        Assert.Equal(
            "a,b;c,d;",
Render(@"
            {%- for x, y in [(""a"", ""b""), (""c"", ""d"")] -%}
                {{- x }},{{ y -}};
            {%- endfor -%}
        ", null, null));
        Assert.Equal(
            "True",
Render("{{ 1 is not string }}", null, null));
        Assert.Equal(
            "ababab",
Render("{{ 'ab' * 3 }}", null, null));
        Assert.Equal(
            "3",
Render("{{ [1, 2, 3][-1] }}", null, null));
        Assert.Equal(
            "OK",
Render("{%- for i in range(0) -%}NAH{% else %}OK{% endfor %}", null, null));
        Assert.Equal(
            "(0, odd),(1, even),(2, odd),(3, even),(4, odd),",
Render(@"
            {%- for i in range(5) -%}
                ({{ i }}, {{ loop.cycle('odd', 'even') }}),
            {%- endfor -%}
        ", null, null));

        Assert.Equal(
        "0, first=True, last=False, index=1, index0=0, revindex=3, revindex0=2, prev=, next=2,\n" +
        "2, first=False, last=False, index=2, index0=1, revindex=2, revindex0=1, prev=0, next=4,\n" +
        "4, first=False, last=True, index=3, index0=2, revindex=1, revindex0=0, prev=2, next=,\n",
Render(
            "{%- for i in range(5) if i % 2 == 0 -%}\n" +
            "{{ i }}, first={{ loop.first }}, last={{ loop.last }}, index={{ loop.index }}, index0={{ loop.index0 }}, revindex={{ loop.revindex }}, revindex0={{ loop.revindex0 }}, prev={{ loop.previtem }}, next={{ loop.nextitem }},\n" +
            "{% endfor -%}",
            null, null
        )
);
        Assert.Equal(
            "[0, 1, 2][0, 2]",
Render(@"
            {%- set o = [0, 1, 2, 3] -%}
            {%- set _ = o.pop() -%}
            {{- o | tojson -}}
            {%- set _ = o.pop(1) -%}
            {{- o | tojson -}}
        ", null, null));
        Assert.Equal(
            @"{""y"": 2}",
Render(@"
            {%- set o = {""x"": 1, ""y"": 2} -%}
            {%- set _ = o.pop(""x"") -%}
            {{- o | tojson -}}
        ", null, null));
        Assert.Equal(
            "&lt;, &gt;, &amp;, &#34;",
Render(@"
            {%- set res = [] -%}
            {%- for c in [""<"", "">"", ""&"", '""'] -%}
                {%- set _ = res.append(c | e) -%}
            {%- endfor -%}
            {{- res | join("", "") -}}
        ", null, null));
        Assert.Equal(
            "x=100, y=2, z=3, w=10",
Render(@"
            {%- set x = 1 -%}
            {%- set y = 2 -%}
            {%- macro foo(x, z, w=10) -%}
                x={{ x }}, y={{ y }}, z={{ z }}, w={{ w -}}
            {%- endmacro -%}
            {{- foo(100, 3) -}}
        ", null, null));
        Assert.Equal(Render(@"
            {% macro input(name, value='', type='text', size=20) -%}
                <input type=""{{ type }}"" name=""{{ name }}"" value=""{{ value|e }}"" size=""{{ size }}"">
            {%- endmacro -%}

            <p>{{ input('username') }}</p>
            <p>{{ input('password', type='password') }}</p>", null, null),
                Parser.NormalizeNewlines(@"
            <p><input type=""text"" name=""username"" value="""" size=""20""></p>
            <p><input type=""password"" name=""password"" value="""" size=""20""></p>"));
        Assert.Equal(
            "[1] [1]",
Render(@"
            {#- The values' default array should be created afresh at each call, unlike the equivalent Python function -#}
            {%- macro foo(values=[]) -%}
                {%- set _ = values.append(1) -%}
                {{- values -}}
            {%- endmacro -%}
            {{- foo() }} {{ foo() -}}", null, null));

        Assert.Equal(
            "x,x",
Render(@"
            {%- macro test() -%}{{ caller() }},{{ caller() }}{%- endmacro -%}
            {%- call test() -%}x{%- endcall -%}
        ", null, null));

        Assert.Equal(
            "Outer[Inner(X)]",
Render(@"
            {%- macro outer() -%}Outer[{{ caller() }}]{%- endmacro -%}
            {%- macro inner() -%}Inner({{ caller() }}){%- endmacro -%}
            {%- call outer() -%}{%- call inner() -%}X{%- endcall -%}{%- endcall -%}
        ", null, null));

        Assert.Equal(
            "<ul><li>A</li><li>B</li></ul>",
Render(@"
            {%- macro test(prefix, suffix) -%}{{ prefix }}{{ caller() }}{{ suffix }}{%- endmacro -%}
            {%- set items = [""a"", ""b""] -%}
            {%- call test(""<ul>"", ""</ul>"") -%}
                {%- for item in items -%}
                    <li>{{ item | upper }}</li>
                {%- endfor -%}
            {%- endcall -%}
        ", null, null));
        Assert.Equal(
            "\\n\\nclass A:\\n  b: 1\\n  c: 2\\n",
Render(@"
        {%- macro recursive(obj) -%}
        {%- set ns = namespace(content = caller()) -%}
        {%- for key, value in obj.items() %}
            {%- if value is mapping %}
                {%- call recursive(value) -%}
                    {{ '\\n\\nclass ' + key.title() + ':\\n' }}
                {%- endcall -%}
            {%- else -%}
                {%- set ns.content = ns.content + '  ' + key + ': ' + value + '\\n' -%}
            {%- endif -%}
        {%- endfor -%}
        {{ ns.content }}
        {%- endmacro -%}

        {%- call recursive({""a"": {""b"": ""1"", ""c"": ""2""}}) -%}
        {%- endcall -%}
    ", null, null));
        Assert.Equal(
            "Foo",
Render("{% generation %}Foo{% endgeneration %}", null, null));
        Assert.Equal(
            "[[1, 2]]",
Render("{{ {1: 2} | items | list | tojson }}", null, null));
        Assert.Equal(
            "[[1, 2], [3, 4], [5, 7]]",
Render("{{ {1: 2, 3: 4, 5: 7} | dictsort | tojson }}", null, null));
        Assert.Equal(
            "[[1, 2]]",
Render(@"{{ {1: 2}.items() | map(""list"") | list }}", null, null));
        Assert.Equal(
            "2; ; 10",
Render("{{ {1: 2}.get(1) }}; {{ {}.get(1) or '' }}; {{ {}.get(1, 10) }}", null, null));
        Assert.Equal(
            @"1,1.2,""a"",true,true,false,false,null,[],[1],[1, 2],{},{""a"": 1},{""1"": ""b""},",
Render(@"
            {%- for x in [1, 1.2, ""a"", true, True, false, False, None, [], [1], [1, 2], {}, {""a"": 1}, {1: ""b""}] -%}
                {{- x | tojson -}},
            {%- endfor -%}
        ", null, null));
        Assert.Equal(
            @"1 """",2 ""Hello""",
Render(@"
            {%- set n = namespace(value=1, title='') -%}
            {{- n.value }} ""{{ n.title }}"",
            {%- set n.value = 2 -%}
            {%- set n.title = 'Hello' -%}
            {{- n.value }} ""{{ n.title }}""", null, null));
        Assert.Equal("[1, 2, 3]", Render("{% set _ = a.b.append(c.d.e) %}{{ a.b }}", new { a = new { b = new int[] { 1, 2 } }, c = new { d = new { e = 3 } } }, null));

        // binding using anonymous type
        Assert.Equal("1,10;2,20;", Render(@"    
{%- for x, y in z -%}
        {{- x }},{{ y -}};
{%- endfor -%}
", new { z = new[] { [1, 10], new int[] { 2, 20 } } }, null));

        // binding using dictionary
        Assert.Equal("1,10;2,20;", Render(@"    
{%- for x, y in z -%}
        {{- x }},{{ y -}};
{%- endfor -%}
", new Dictionary<string, object>() { { "z", new[] { [1, 10], new int[] { 2, 20 } } } }, null));

        // binding using explicit type
        Assert.Equal("1,10;2,20;", Render(@"    
{%- for x, y in z -%}
        {{- x }},{{ y -}};
{%- endfor -%}
", new TestBinding1(), null));

        // binding using JsonElement
        Assert.Equal("1,10;2,20;", Render(@"    
{%- for x, y in z -%}
        {{- x }},{{ y -}};
{%- endfor -%}
", JsonSerializer.SerializeToElement(new TestBinding1()), null));

        // binding using JSon string
        Assert.Equal("1,10;2,20;", Render(@"    
{%- for x, y in z -%}
        {{- x }},{{ y -}};
{%- endfor -%}
", JsonSerializer.Deserialize<JsonElement>("{\"z\":[[1,10],[2,20]]}"), null));

        Assert.Equal(
            " a bc ",
Render(" a {{  'b' -}} c ", null, null));
        Assert.Equal(
            " ab c ",
Render(" a {{- 'b'  }} c ", null, null));
        Assert.Equal(
            "ab\nc",
Render("a\n{{- 'b'  }}\nc", null, null));
        Assert.Equal(
            "a\nbc",
Render("a\n{{  'b' -}}\nc", null, null));
        Assert.Equal(
            "True",
Render("{{ [] is iterable }}", null, null));
        Assert.Equal(
            "True",
Render("{{ [] is not number }}", null, null));
        Assert.Equal(
            "[1, 2, 3][0, 1][1, 2]",
Render("{% set x = [0, 1, 2, 3] %}{{ x[1:] }}{{ x[:2] }}{{ x[1:3] }}", null, null));
        Assert.Equal(
            "123;01;12;0123;0123",
Render("{% set x = '0123' %}{{ x[1:] }};{{ x[:2] }};{{ x[1:3] }};{{ x[:] }};{{ x[::] }}", null, null));
        Assert.Equal(
            "[3, 2, 1, 0][3, 2, 1][2, 1, 0][2, 1][0, 2][3, 1][2, 0]",
Render("{% set x = [0, 1, 2, 3] %}{{ x[::-1] }}{{ x[:0:-1] }}{{ x[2::-1] }}{{ x[2:0:-1] }}{{ x[::2] }}{{ x[::-2] }}{{ x[-2::-2] }}", null, null));
        Assert.Equal(
            "3210;321;210;21;02;31;20",
Render("{% set x = '0123' %}{{ x[::-1] }};{{ x[:0:-1] }};{{ x[2::-1] }};{{ x[2:0:-1] }};{{ x[::2] }};{{ x[::-2] }};{{ x[-2::-2] }}", null, null));
        Assert.Equal(
            "a",
Render("{{ ' a  ' | trim }}", null, null));
        Assert.Equal(
            "",
Render("{{ None | trim }}", null, null));
        Assert.Equal(
            "[0, 1, 2][4, 5, 6][0, 2, 4, 6, 8]",
Render("{{ range(3) | list }}{{ range(4, 7) | list }}{{ range(0, 10, 2) | list }}", null, null));
        Assert.Equal(
            " abc ",
Render(@" {{ ""a"" -}} b {{- ""c"" }} ", null, null));
        Assert.Equal(
            "[\n  1\n]",
Render("{% set x = [] %}{% set _ = x.append(1) %}{{ x | tojson(indent=2) }}", null, null));
        Assert.Equal(
            "True",
Render("{{ not [] }}", null, null));
        Assert.Equal("True", Render("{{ tool.function.name == 'ipython' }}", new { tool = new { function = new { name = "ipython" } } }, null));
        Assert.Equal(
            "Hello Olivier",
Render(@"
        {%- set user = ""Olivier"" -%}
        {%- set greeting = ""Hello "" ~ user -%}
        {{- greeting -}}
    ", null, null));

        Assert.Equal(
            "",
Render("{% if 1 %}{% elif 1 %}{% else %}{% endif %}", null, null));

        Assert.Equal(
            "0,1,2,",
Render("{% for i in range(10) %}{{ i }},{% if i == 2 %}{% break %}{% endif %}{% endfor %}", null, null));
        Assert.Equal(
            "0,2,4,6,8,",
Render("{% for i in range(10) %}{% if i % 2 %}{% continue %}{% endif %}{{ i }},{% endfor %}", null, null));

        Assert.Equal("3", Render("{{ (a.b.c) }}", new { a = new { b = new { c = 3 } } }, null));

        Assert.True(DateTimeOffset.TryParseExact(Render("{{ strftime_now(\"%Y-%m-%d %H:%M:%S\") }}", null, null), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var _));
        Assert.True(DateTimeOffset.TryParseExact(Render("{{ strftime_now(\"%A, %B %d, %Y\") }}", null, null), "dddd, MMMM dd, yyyy", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var _));   
    }

    private static void Test(Action testCode, string expectedExceptionMessageSubstring)
    {
        Test<JinjaException>(testCode, expectedExceptionMessageSubstring);
    }

    private static void Test<T>(Action testCode, string expectedExceptionMessageSubstring)
        where T : JinjaException
    {
        var exception = Assert.Throws<T>(testCode);
        Assert.Contains(expectedExceptionMessageSubstring, exception.Message);
    }

    [Fact]
    public void ExceptionTests()
    {
        // Items can only be called on mappings
        Test(() => Render("{{ '' | items }}", null, null),
            "Can only get item pairs from a mapping");
        Test(() => Render("{{ [] | items }}", null, null),
            "Can only get item pairs from a mapping");
        Test(() => Render("{{ None | items }}", null, null),
            "Can only get item pairs from a mapping");

        // break/continue outside of a loop
        Test<LoopControlException>(() => Render("{% break %}", null, null),
            "break outside of a loop");
        Test<LoopControlException>(() => Render("{% continue %}", null, null),
            "continue outside of a loop");

        // pop from empty list/dict
        Test(() => Render("{%- set _ = [].pop() -%}", null, null),
            "pop from empty list");
        Test(() => Render("{%- set _ = {}.pop() -%}", null, null),
            "pop");
        Test(() => Render("{%- set _ = {}.pop('foooo') -%}", null, null),
            "foooo");

        // Unexpected control tags
        Test(() => Render("{% else %}", null, null),
            "Unexpected else");
        Test(() => Render("{% endif %}", null, null),
            "Unexpected endif");
        Test(() => Render("{% elif 1 %}", null, null),
            "Unexpected elif");
        Test(() => Render("{% endfor %}", null, null),
            "Unexpected endfor");
        Test(() => Render("{% endfilter %}", null, null),
            "Unexpected endfilter");
        Test(() => Render("{% endmacro %}", null, null),
            "Unexpected endmacro");
        Test(() => Render("{% endcall %}", null, null),
            "Unexpected endcall");

        // Unterminated blocks
        Test(() => Render("{% if 1 %}", null, null),
            "Unterminated if");
        Test(() => Render("{% for x in 1 %}", null, null),
            "Unterminated for");
        Test(() => Render("{% generation %}", null, null),
            "Unterminated generation");
        Test(() => Render("{% if 1 %}{% else %}", null, null),
            "Unterminated if");
        Test(() => Render("{% if 1 %}{% else %}{% elif 1 %}{% endif %}", null, null),
            "Unterminated if");
        Test(() => Render("{% filter trim %}", null, null),
            "Unterminated filter");
        Test(() => Render("{# ", null, null),
            "Missing end of comment tag");
        Test(() => Render("{% macro test() %}", null, null),
            "Unterminated macro");
        Test(() => Render("{% call test %}", null, null),
            "Unterminated call");

        // Invalid call block syntax
        Test(() => Render("{%- macro test() -%}content{%- endmacro -%}{%- call test -%}caller_content{%- endcall -%}", null, null),
            "Invalid call block syntax - expected function call");
    }
}