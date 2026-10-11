"""Convert the browser-rendered original emblem; no external assets."""
from pathlib import Path
import sys
from PIL import Image

source = Path(sys.argv[1])
web = Path(sys.argv[2])
brand = web / 'images' / 'brand'
with Image.open(source) as image:
    rgba = image.convert('RGBA')
    rgba.save(web / 'favicon.ico', format='ICO', sizes=[(16, 16), (32, 32), (48, 48), (64, 64)])
    for name, size in [('favicon-32x32.png', 32), ('apple-touch-icon.png', 180)]:
        rgba.resize((size, size), Image.Resampling.LANCZOS).save(brand / name, optimize=True)
print('Generated favicon.ico (16/32/48/64), PNG 32 and Apple icon 180 from unchanged local SVG.')
