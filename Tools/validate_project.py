from pathlib import Path
import json,re,sys
root=Path(__file__).resolve().parents[1]
errors=[]
for n in range(1,51):
 p=root/f'Assets/Resources/Levels/level_{n:03}.json'
 try:
  d=json.loads(p.read_text(encoding='utf-8'))
  if d['id']!=n or d['inkLimit']<=0: errors.append(str(p))
 except Exception as e: errors.append(f'{p}: {e}')
for p in root.rglob('*.cs'):
 s=p.read_text(encoding='utf-8'); depth=0
 for ch in re.sub(r'"(?:\\.|[^"\\])*"','""',s):
  if ch=='{': depth+=1
  elif ch=='}': depth-=1
  if depth<0: errors.append(f'unbalanced {p}');break
 if depth: errors.append(f'unbalanced {p}: {depth}')
forbidden=['b.sharedMaterial=','CompareTag(\"Dynamic\")']
for token in forbidden:
 for p in root.rglob('*.cs'):
  if token in p.read_text(encoding='utf-8'): errors.append(f'forbidden {token} in {p}')
required=['Assets/Editor/AndroidBuild.cs','.github/workflows/android-build.yml','ProjectSettings/ProjectVersion.txt']
for r in required:
 if not (root/r).exists():errors.append('missing '+r)
print('VALIDATION_OK' if not errors else '\n'.join(errors));sys.exit(bool(errors))
