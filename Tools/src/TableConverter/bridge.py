import sys
import os
import argparse

sys.path.insert(0, os.path.dirname(__file__))
from xlsx2dsv import xlsx_to_dsv
from dsvHeader2cs import process_dsv
from csContainerGenerator import collect_table_info, generate_container


def run(xlsx_dir, dsv_dir, cs_dir, exclude_prefix, delimiter):
    # Step 1: xlsx -> dsv
    xlsx_files = [
        os.path.join(xlsx_dir, f)
        for f in os.listdir(xlsx_dir)
        if f.endswith('.xlsx')
    ]

    if not xlsx_files:
        print(f"[WARN] No .xlsx files found in '{xlsx_dir}'.")
        sys.exit(0)

    os.makedirs(dsv_dir, exist_ok=True)
    for f in os.listdir(dsv_dir):
        if not f.endswith('.tsv'):
            continue
        fp = os.path.join(dsv_dir, f)
        if os.path.isfile(fp):
            os.remove(fp)
    print(f"[OK] Cleared: {dsv_dir}")

    for xlsx_path in xlsx_files:
        print(f"[--] Processing: {xlsx_path}")
        xlsx_to_dsv(xlsx_path, dsv_dir, exclude_prefix, delimiter, append=True)

    # Step 2: dsv -> cs
    dsv_files = [
        os.path.join(dsv_dir, f)
        for f in os.listdir(dsv_dir)
        if f.endswith('.tsv')
    ]

    if not dsv_files:
        print(f"[WARN] No .dsv files found in '{dsv_dir}'.")
        sys.exit(0)

    for dsv_path in dsv_files:
        process_dsv(dsv_path, cs_dir, delimiter)

    # Step 3: cs -> TableContainer.cs
    tables = collect_table_info(cs_dir)
    if not tables:
        print(f"[WARN] No valid table classes found in '{cs_dir}'.")
        sys.exit(0)

    output_path = os.path.join(cs_dir, 'TableContainer.cs')
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(generate_container(tables))
    print(f"[OK] {len(tables)} tables -> {output_path}")


def main():
    parser = argparse.ArgumentParser(description='xlsx -> dsv -> cs pipeline')
    parser.add_argument('xlsx_dir',             help='Input directory containing .xlsx files')
    parser.add_argument('dsv_dir',              help='Intermediate DSV output directory')
    parser.add_argument('cs_dir',               help='C# output directory')
    parser.add_argument('exclude_prefix',       help='Exclude sheets whose name starts with this text')
    parser.add_argument('-d', '--delimiter',    default='\t',
                        help='Delimiter character (default: tab)')
    args = parser.parse_args()

    delimiter = args.delimiter.encode().decode('unicode_escape')

    run(args.xlsx_dir, args.dsv_dir, args.cs_dir, args.exclude_prefix, delimiter)


if __name__ == '__main__':
    main()
