#!/bin/bash
sed -i "s/TestKeyGenForMakeingCustomShitForSecurityPurposses/${JWT_SECRET//\//\\/}/g" appsettings.json;
./CliChat