#!/bin/bash

RIMWORLD_PATH="/H/SteamLibrary/steamapps/common/RimWorld/Mods/"
MOD_NAME="SexualityTraits"



DEBUG=true
MOD_PATH="$RIMWORLD_PATH""$MOD_NAME"
BIN_PATH="./Sources/SexualityTraits/bin/1.4/"

if [[ $DEBUG -eq "false" ]]; then
    BIN_PATH=$BIN_PATH"Debug/"
else
    BIN_PATH=$BIN_PATH"Realse/"
fi

BIN_PATH=$BIN_PATH"SexualityTraits.dll"

mkdir -p $MOD_PATH

cp $BIN_PATH "1.4/Assemblies"

cp -r "1.4" $MOD_PATH
cp -r "About" $MOD_PATH
cp -r "Languages" $MOD_PATH



echo "Successfully deployed to $MOD_PATH"
