//-----------------------------------------------------------------------------
// SkinnedModel.fx
//
// Microsoft Game Technology Group
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Maximum number of bone matrices we can render using shader 2.0 in a single pass.
// If you change this, update SkinnedModelProcessor.cs to match.
#define MaxBones 59


// Input parameters.
float4 Highlight;

float4x4 View;
float4x4 Projection;

float4x4 Bones[MaxBones];
float4x4 World;
float Alpha;
bool Animating;
float3 Light1Direction = normalize(float3(1, 1, -2));
float3 Light1Color = float3(1.9, 1.8, 1.7);

float3 Light2Direction = normalize(float3(-1, -1, 1));
float3 Light2Color = float3(0.1, 1.0, 0.8);

float3 AmbientColor = 0.3;

float3 CameraPos;
float3 LightPosition;


float LightPower;
float Ambient;
texture Texture;
texture NormTexture;


sampler Sampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
    
    
};
sampler NormSampler = sampler_state
{
    Texture = (NormTexture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    
    
};


// Vertex shader input structure.
struct VS_INPUT
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
};


// Vertex shader output structure.
struct VS_OUTPUT
{
    float4 Position : POSITION0;
   
    float2 TexCoord : TEXCOORD0;
     float3 Normal : TEXCOORD1;
	float3 WorldPos : TEXCOORD2;
};


// Vertex shader program.
VS_OUTPUT VertexShader(VS_INPUT input)
{
    VS_OUTPUT output;
    float3 normal;
    if(Animating)
    {
    // Blend between the weighted bone matrices.
    float4x4 skinTransform = 0;
    
    skinTransform += Bones[input.BoneIndices.x] * input.BoneWeights.x;
    skinTransform += Bones[input.BoneIndices.y] * input.BoneWeights.y;
    skinTransform += Bones[input.BoneIndices.z] * input.BoneWeights.z;
    skinTransform += Bones[input.BoneIndices.w] * input.BoneWeights.w;
    
    // Skin the vertex position.
    skinTransform = mul(skinTransform, World);
    float4 position = mul(input.Position, skinTransform);
  
    output.Position = mul(mul(position, View), Projection);

    // Skin the vertex normal, then compute lighting.
     normal = normalize(mul(input.Normal, skinTransform));
     // output.WorldPos = position;
    
    }
    else
    {
   float4 position = mul(input.Position, World);
    output.Position = mul(mul(position, View), Projection);
     normal = normalize(mul(input.Normal, World)); 
  
   }
    
 output.Normal = normal;
   
    output.TexCoord = input.TexCoord;
  
  output.WorldPos = mul(input.Position, World);
    return output;
}


// Pixel shader input structure.
struct PS_INPUT
{
    float4 Position : POSITION0;
   
    float2 TexCoord : TEXCOORD0;
     float3 Normal : TEXCOORD1;
	float3 WorldPos : TEXCOORD2;
};
float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(lightPos - pos3D);
    return dot(lightDir, normal);    
}

// Pixel shader program.
float4 PixelShader(PS_INPUT input) : COLOR0
{
 
	float diffuseLightingFactor = DotProduct(LightPosition, input.WorldPos, input.Normal);
	//diffuseLightingFactor = saturate(diffuseLightingFactor);
	diffuseLightingFactor *= LightPower;
 
 float4 baseColor = tex2D(Sampler, input.TexCoord);

    if(baseColor.a < 1)
		baseColor = Highlight;
	//else
	{
		baseColor *= (diffuseLightingFactor + Ambient); //* Highlight;	
	}
		baseColor.a = Alpha;
 return baseColor;
 
}


technique SkinnedModelTechnique
{
    pass SkinnedModelPass
    {
        VertexShader = compile vs_2_0 VertexShader();
        PixelShader = compile ps_2_0 PixelShader();
    }
}
