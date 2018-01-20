https://github.com/svg/svgo
Step 1: install nodejs in Libraries/x64/archive/node-v6.9.4-x64.msi
Step 2: open cmd type: "npm install -g svgo"
Done
User:
	svgo test.svg
	svgo test.svg test.min.svg

	svgo "C:\Users\Top\Desktop\kiwi.svg" "C:\Users\Top\Desktop\kiwi1.svg" --quiet --indent=0 --enable=removeComments --enable=cleanupIDs --enable=cleanupAttrs --enable=removeEmptyText --enable=removeHiddenElems --enable=removeMetadata --enable=removeDesc --enable=removeXMLProcInst --enable=removeDoctype --enable=removeViewBox --enable=removeViewBox

//-------------------------------------------------------------------------------------------------
https://github.com/scour-project/scour

Step 1: install python in Libraries/x86/archive/python-3.6.0.exe
- select install pip - libraries like nuget package
- or can manual install pip by https://packaging.python.org/installing/
- "python -m pip install -U pip setuptools"

Step 2: Start cmd as admin then "pip install scour"
- or can manuall install scour after install python by cmd as admin: 
	"pip install https://github.com/codedread/scour/archive/master.zip" (url)
- or "pip install <part to scour-master.zip in archive folder>

Done, can open cmd and use scour