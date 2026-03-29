import sys
import os
import re
import argparse

ID_FIELD_RE = re.compile(r'public\s+(\w+(?:\[\])?)\s+ID\s*;')


def collect_table_info(cs_dir):
    tables = []
    for fname in sorted(os.listdir(cs_dir)):
        if fname == 'TableContainer.cs' or not fname.endswith('.cs'):
            continue
        class_name = fname[:-3]
        fpath = os.path.join(cs_dir, fname)
        with open(fpath, 'r', encoding='utf-8') as f:
            content = f.read()
        m = ID_FIELD_RE.search(content)
        if m is None:
            print(f"[WARN] '{class_name}': no 'ID' field found, skipping.")
            continue
        id_type = m.group(1)
        tables.append((class_name, id_type))
    return tables


def generate_container(tables):
    i1 = '    '
    i2 = '        '
    i3 = '            '
    Dict = 'System.Collections.Generic.Dictionary'

    lines = []
    lines.append('public partial class TableContainer')
    lines.append('{')

    # Fields
    for class_name, id_type in tables:
        lines.append(f'{i1}public {Dict}<{id_type}, {class_name}> Table_{class_name};')

    lines.append('')

    # Load<ClassName> methods
    for class_name, id_type in tables:
        lines.append(f'{i1}public void Load{class_name}({class_name}[] items)')
        lines.append(f'{i1}{{')
        lines.append(f'{i2}Table_{class_name} = new {Dict}<{id_type}, {class_name}>();')
        lines.append(f'{i2}foreach (var item in items)')
        lines.append(f'{i2}{{')
        lines.append(f'{i3}Table_{class_name}[item.ID] = item;')
        lines.append(f'{i2}}}')
        lines.append(f'{i1}}}')
        lines.append('')

    # Load(string folderPath) method
    lines.append(f'{i1}public void Load(string folderPath)')
    lines.append(f'{i1}{{')
    for class_name, _ in tables:
        tsv_path = f'System.IO.Path.Combine(folderPath, "{class_name}.tsv")'
        lines.append(f'{i2}Load{class_name}({class_name}.Read(System.IO.File.ReadAllText({tsv_path})));')
    lines.append(f'{i1}}}')

    lines.append('}')

    return '\n'.join(lines) + '\n'


def main():
    parser = argparse.ArgumentParser(description='Generate TableContainer.cs from C# table class files.')
    parser.add_argument('cs_dir', help='Directory containing generated .cs table files')
    args = parser.parse_args()

    if not os.path.isdir(args.cs_dir):
        print(f"[ERROR] Not a directory: {args.cs_dir}")
        sys.exit(1)

    tables = collect_table_info(args.cs_dir)

    if not tables:
        print("[WARN] No valid table classes found.")
        sys.exit(0)

    cs_code = generate_container(tables)

    output_path = os.path.join(args.cs_dir, 'TableContainer.cs')
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(cs_code)

    print(f"[OK] {len(tables)} tables -> {output_path}")
    for class_name, id_type in tables:
        print(f"     {class_name} (ID: {id_type})")


if __name__ == '__main__':
    main()
