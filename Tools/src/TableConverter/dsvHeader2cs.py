import sys
import os
import argparse

HEADER_ROWS = 2

TYPE_MAP = {
    'int':         'int',
    'string':      'string',
    'float':       'float',
    'long':        'long',
    'int-list':    'int[]',
    'string-list': 'string[]',
    'float-list':  'float[]',
    'long-list':   'long[]',
}


def cs_parse_expr(cs_type, col_expr):
    if cs_type == 'int':
        return f'int.Parse({col_expr})'
    elif cs_type == 'string':
        return col_expr
    elif cs_type == 'float':
        return f'float.Parse({col_expr}, System.Globalization.CultureInfo.InvariantCulture)'
    elif cs_type == 'long':
        return f'long.Parse({col_expr})'
    elif cs_type == 'int[]':
        return f'System.Array.ConvertAll({col_expr}.Split(\',\'), int.Parse)'
    elif cs_type == 'string[]':
        return f'{col_expr}.Split(\',\')'
    elif cs_type == 'float[]':
        return f'System.Array.ConvertAll({col_expr}.Split(\',\'), s => float.Parse(s, System.Globalization.CultureInfo.InvariantCulture))'
    elif cs_type == 'long[]':
        return f'System.Array.ConvertAll({col_expr}.Split(\',\'), long.Parse)'


def generate_cs(class_name, fields):
    i1 = '    '
    i2 = '        '
    i3 = '            '

    lines = []
    lines.append(f'public class {class_name}')
    lines.append('{')

    for cs_type, field_name in fields:
        lines.append(f'{i1}public {cs_type} {field_name};')

    lines.append('')
    lines.append(f"{i1}public static {class_name}[] Read(string dsv, char delimiter = '\\t')")
    lines.append(f'{i1}{{')
    lines.append(f"{i2}var lines = dsv.Split('\\n');")
    lines.append(f'{i2}var results = new System.Collections.Generic.List<{class_name}>();')
    lines.append(f'{i2}for (int i = {HEADER_ROWS}; i < lines.Length; i++)')
    lines.append(f'{i2}{{')
    lines.append(f'{i3}var line = lines[i].Trim();')
    lines.append(f'{i3}if (string.IsNullOrEmpty(line)) continue;')
    lines.append(f'{i3}var cols = line.Split(delimiter);')
    lines.append(f'{i3}var obj = new {class_name}();')

    for idx, (cs_type, field_name) in enumerate(fields):
        expr = cs_parse_expr(cs_type, f'cols[{idx}]')
        lines.append(f'{i3}obj.{field_name} = {expr};')

    lines.append(f'{i3}results.Add(obj);')
    lines.append(f'{i2}}}')
    lines.append(f'{i2}return results.ToArray();')
    lines.append(f'{i1}}}')
    lines.append('}')

    return '\n'.join(lines) + '\n'


def process_dsv(input_path, output_dir, delimiter):
    class_name = os.path.splitext(os.path.basename(input_path))[0]

    with open(input_path, 'r', encoding='utf-8') as f:
        lines = f.read().splitlines()

    if len(lines) < HEADER_ROWS:
        print(f"[ERROR] '{input_path}': fewer than {HEADER_ROWS} header rows.")
        return

    type_row = lines[0].split(delimiter)
    name_row = lines[1].split(delimiter)

    fields = []
    for raw_type, field_name in zip(type_row, name_row):
        raw_type   = raw_type.strip()
        field_name = field_name.strip()
        if not raw_type or not field_name:
            continue
        cs_type = TYPE_MAP.get(raw_type)
        if cs_type is None:
            print(f"[WARN] '{class_name}': unknown type '{raw_type}' for field '{field_name}', skipping.")
            continue
        fields.append((cs_type, field_name))

    if not fields:
        print(f"[ERROR] '{input_path}': no valid fields found.")
        return

    cs_code = generate_cs(class_name, fields)

    os.makedirs(output_dir, exist_ok=True)
    output_path = os.path.join(output_dir, class_name + '.cs')

    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(cs_code)

    print(f"[OK] '{class_name}' -> {output_path}")


def main():
    parser = argparse.ArgumentParser(description='Generate C# class definitions from DSV header rows.')
    parser.add_argument('input',             help='Input directory containing .dsv files')
    parser.add_argument('output',            help='Output directory for .cs files')
    parser.add_argument('-d', '--delimiter', default='\t',
                        help='Delimiter used in DSV files (default: tab)')
    args = parser.parse_args()

    if not os.path.isdir(args.input):
        print(f"[ERROR] Input path is not a directory: {args.input}")
        sys.exit(1)

    dsv_files = [
        os.path.join(args.input, f)
        for f in os.listdir(args.input)
        if f.endswith('.tsv')
    ]

    if not dsv_files:
        print(f"[WARN] No .dsv files found in '{args.input}'.")
        sys.exit(0)

    delimiter = args.delimiter.encode().decode('unicode_escape')

    for input_path in dsv_files:
        process_dsv(input_path, args.output, delimiter)


if __name__ == '__main__':
    main()
