/* Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

namespace Gibbed.EFX.FileFormats
{
    public enum ElementType : byte
    {
        None = 0, // TsElementType_NONE
        Point = 1, // !! TsElementType_POINT
        Sprite = 2, // !! TsElementType_SPRITE
        Line = 3, // !! TsElementType_LINE
        Circle = 4, // TsElementType_CIRCLE
        Model = 5, // !! TsElementType_MODEL
        Column = 6, // TsElementType_COLUMN
        Sphere = 7, // TsElementType_SPHERE
        Board1 = 8, // TsElementType_BOARD1
        
        Tail = 10, // TsElementType_TAIL
        
        Light = 64, // TsElementType_LIGHT
        ObjectFog = 65, // !! TsElementType_OBJFOG
        CameraVibration = 66, // TsElementType_CAMERAVIBRATION
        
        VThunder = 96, // TsElementType_VTHUNDER
        VThunder2 = 97, // TsElementType_VTHUNDER2
        VThunder3 = 98, // TsElementType_VTHUNDER3
        VLaser = 99, // TsElementType_VLASER
        VLaser2 = 100, // TsElementType_VLASER2
        VTextureThunder = 101, // TsElementType_VTEXTHUNDER
        VTextureThunder2 = 102, // !! TsElementType_VTEXTHUNDER2
        TsElementType_VAFTERWIND = 103, // !! TsElementType_VAFTERWIND
        NhElementType_VTEXLASER = 104, // NhElementType_VTEXLASER
        NhElementType_VEFFAFTERGLOW = 105, // NhElementType_VEFFAFTERGLOW
        NhElementType_VEFFWATER = 106, // NhElementType_VEFFWATER
        NhElementType_VAFTERWIND_MULTEX = 107, // !! NhElementType_VAFTERWIND_MULTEX

        NhElementType_VEFFMETABALL = 112, // NhElementType_VEFFMETABALL
        NhElementType_VAFTERTHUNDER = 113, // NhElementType_VAFTERTHUNDER
        NhElementType_VPHYSICALEFFECT1 = 114, // NhElementType_VPHYSICALEFFECT1
        NhElementType_GLASSBREAK = 115, // NhElementType_GLASSBREAK
        NhElementType_MODELBREAK = 116, // NhElementType_MODELBREAK

        TsElementType_VLINE1 = 128, // TsElementType_VLINE1
        TsElementType_VBEZIER1 = 129, // TsElementType_VBEZIER1
        TsElementType_VLCYLINDER1 = 130, // TsElementType_VLCYLINDER1
        TsElementType_VLCYLINDER2 = 131, // TsElementType_VLCYLINDER2
        TsElementType_VGLOBULARANM = 132, // TsElementType_VGLOBULARANM
        TsElementType_VTAIL = 133, // TsElementType_VTAIL
        TsElementType_VCLROTLINE = 134, // TsElementType_VCLROTLINE
        TsElementType_VGLROTLINE = 135, // TsElementType_VGLROTLINE
        TsElementType_VGLROTANM = 136, // TsElementType_VGLROTANM
        TsElementType_VCLROTANM = 137, // TsElementType_VCLROTANM
        TsElementType_VGLANM2 = 138, // TsElementType_VGLANM2
        TsElementType_VCCFITANM = 139, // TsElementType_VCCFITANM
        TsElementType_VMODEL02 = 140, // TsElementType_VMODEL02
        TsElementType_VMODEL00 = 141, // TsElementType_VMODEL00
        TsElementType_VMODEL01 = 142, // TsElementType_VMODEL01
        TsElementType_VMODEL03 = 143, // TsElementType_VMODEL03
        TsElementType_VMODEL04 = 144, // TsElementType_VMODEL04
        TsElementType_VMODEL05 = 145, // TsElementType_VMODEL05
        TsElementType_VMODEL06 = 146, // TsElementType_VMODEL06
        TsElementType_VMODEL07 = 147, // TsElementType_VMODEL07
        TsElementType_VMODEL08 = 148, // TsElementType_VMODEL08
        TsElementType_VMODEL09 = 149, // TsElementType_VMODEL09
        TsElementType_VGLANM3 = 150, // TsElementType_VGLANM3
        TsElementType_VMODEL10 = 151, // TsElementType_VMODEL10

        TsElementType_2DSTART = 160, // TsElementType_2DSTART
        TsElementType_VLENSFLARE = 161, // TsElementType_VLENSFLARE
        TsElementType_2DTEX = 162, // !! TsElementType_2DTEX
        NhElementType_VRAIN2 = 163, // NhElementType_VRAIN2
        NhElementType_LOCATIONTITLE = 164, // NhElementType_LOCATIONTITLE
        TsElementType_2DTEX2 = 165, // !! TsElementType_2DTEX2
        NhElementType_2DTEX2 = 166, // !! NhElementType_2DTEX2
        NhElementType_2DLINEBLUR = 167, // NhElementType_2DLINEBLUR
        NhElementType_2DTEX3 = 168, // NhElementType_2DTEX3
        NhElementType_2DCIRCLEPOLY = 169, // NhElementType_2DCIRCLEPOLY
        NhElementType_2DLINEBLUR2 = 170, // NhElementType_2DLINEBLUR2
        NhElementType_2DTEXPOLYREQU = 171, // NhElementType_2DTEXPOLYREQU

        ExtCtrl = 208, // !! TsElementType_EXTCTRL
        PCamera = 209, // !! TsElementType_PCAMERA

        PadVibration = 240, // TsElementType_PADVIBRATION
        Sound = 241, // TsElementType_SOUND

        Invalid = 0xFF, // TsElementType_INVALID
    }
}
