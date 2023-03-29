FeatureScript 1993;

import(path : "onshape/std/debug.fs", version : "1993.0");
import(path : "onshape/std/frame.fs", version : "1993.0");
import(path : "onshape/std/endcap.fs", version : "1993.0");
import(path : "onshape/std/booleanoperationtype.gen.fs", version : "1993.0");
import(path : "onshape/std/cutlistMath.fs", version : "1993.0");
import(path : "onshape/std/deleteBodies.fs", version : "1993.0");
import(path : "onshape/std/error.fs", version : "1993.0");
import(path : "onshape/std/feature.fs", version : "1993.0");
import(path : "onshape/std/frameAttributes.fs", version : "1993.0");
import(path : "onshape/std/frameUtils.fs", version : "1993.0");
import(path : "onshape/std/table.fs", version : "1993.0");
import(path : "onshape/std/topologyUtils.fs", version : "1993.0");
import(path : "onshape/std/transform.fs", version : "1993.0");


annotation { "Feature Type Name" : "Set Weldment BOM Properties" }
export const weldmentBOM = defineFeature(function(context is Context, id is Id, definition is map)
    precondition
    {
    }
    {
        var rows = [];
        var cols = undefined;
        var partNo = 0;
        const composites = qFrameCutlist(qBodyType(qEverything(EntityType.BODY), BodyType.COMPOSITE));
        for (var composite in evaluateQuery(context, composites))
        {
            const attribute = getCutlistAttribute(context, composite);
            verify(attribute != undefined, "Can't find cutlist table for composite part");
            var currentTable = attribute.table;
            
            if (cols == undefined)
            {
                cols = currentTable["columnDefinitions"];
            }
            
            for (var r in currentTable["rows"])
            {
                partNo += 1;
                var subNo = 0;
                for (var frame in evaluateQuery(context, r.entities))
                {
                    setProperty(context, {
                            "entities" : frame,
                            "propertyType" : PropertyType.PART_NUMBER,
                            "value" :  "" ~ partNo
                    });
                    
                    subNo += 1;
                    setProperty(context, {
                            "entities" : frame,
                            "propertyType" : PropertyType.NAME,
                            "value" :  "Part " ~ partNo ~ "-" ~ subNo
                    });
                    
                    setProperty(context, {
                            "entities" : frame,
                            "propertyType" : PropertyType.DESCRIPTION,
                            "value" : toString(r["columnIdToCell"]["Description"])
                    });
                    
                    if (r["columnIdToCell"]["Description"] == undefined 
                            || r["columnIdToCell"]["Description"] == "Not Set" 
                            || r["columnIdToCell"]["Description"] == "")
                    {
                        continue;
                    }

                    setProperty(context, {
                            "entities" : frame,
                            "propertyType" : PropertyType.TITLE_3,
                            "value" : toString(r["columnIdToCell"]["Length"])
                    });
                }
            }
        }
    });

annotation { "Table Type Name" : "Weldment BOM" }
export const vr3Table = defineTable(function(context is Context, definition is map) returns Table
    precondition
    {
    }
    {
        var rows = [];
        var cols = undefined;
        var partNo = 0;
        const composites = qFrameCutlist(qBodyType(qEverything(EntityType.BODY), BodyType.COMPOSITE));
        for (var composite in evaluateQuery(context, composites))
        {
            const attribute = getCutlistAttribute(context, composite);
            verify(attribute != undefined, "Can't find cutlist table for composite part");
            var currentTable = attribute.table;
            
            if (cols == undefined)
            {
                cols = currentTable["columnDefinitions"];
            }
            
            for (var r in currentTable["rows"])
            {
                partNo += 1;
                r.columnIdToCell[CUTLIST_ITEM] = "" ~ partNo;
                rows = append(rows, r);
            }
        }
        
        return table("Weldment BOM", cols, rows);
    });
