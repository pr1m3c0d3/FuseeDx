// Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Original code from SlimDX project.
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------
/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Data;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace MiniTri
{
    /// <summary>
    ///   SharpDX port of SharpDX-MiniTri Direct3D 11 Sample
    /// </summary>
    /// 

    [StructLayout(LayoutKind.Sequential)]
    internal struct Variables
    {
        public float R, G, B, A;
    }
   [StructLayout(LayoutKind.Sequential)]
    internal struct SharpDxShaderParamInfo
    {
       public PixelShader _ps;
       public VertexShader _vs;
       public ShaderBytecode _psByteCode;
       public ShaderBytecode _vsByteCode;
       public string _buffername;
       public int _varCount;
       public string _varName;
       public int _varSize;
       public int _varPositionB;
       public ShaderVariableFlags _flags;
       public int _varPositionI;
       public Buffer _sdxBuffer;
       public ShaderVariableType _varType;
       public DataStream _bufferParams;
       public ShaderReflectionVariable _varValue;
    }

    internal static class Program
    {
        internal static ShaderDescription sDesc;
        internal static ShaderReflection pRefelector;
        private static Buffer _buff;


        [STAThread]
        private static unsafe void Main()
        {

            var form = new RenderForm("SharpDX - MiniTri Direct3D 11 Sample TEST!!!!!!!!!");

            // SwapChain description
            var desc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription =
                        new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                            new Rational(60, 1), Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = form.Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                };

            // Create Device and SwapChain
            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);
            var context = device.ImmediateContext;

            // Ignore all windows events
            var factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);

            // Compile Vertex and Pixel shaders
            var vertexShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.fx", "VS", "vs_4_0", ShaderFlags.None,
                                                                      EffectFlags.None);
            var vertexShader = new VertexShader(device, vertexShaderByteCode);

            var pixelShaderByteCode = ShaderBytecode.CompileFromFile("MiniTri.fx", "PS", "ps_4_0", ShaderFlags.None,
                                                                     EffectFlags.None);
            var pixelShader = new PixelShader(device, pixelShaderByteCode);




            //Layout Color
            // Layout from VertexShader input signature
            var layout = new InputLayout(
                device,
                ShaderSignature.GetInputSignature(vertexShaderByteCode),
                new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 1)
                    });

            // Instantiate Vertex buiffer from vertex data
            var vertices = Buffer.Create(device, BindFlags.VertexBuffer, new[]
                {
                    new Vector3(-0.9f, -0.2f, 0.0f),
                    new Vector3(0.0f, 0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f)

                });
            // Instantiate Vertex buiffer from vertex data
            var verticesColors = Buffer.Create(device, BindFlags.VertexBuffer, new[]
                {
                    new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                    new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                });
            var triangles = Buffer.Create(device, BindFlags.IndexBuffer, new short[]
                {
                    1,
                    2,
                    3
                });



            // Prepare All the stages
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[]
                {
                    new VertexBufferBinding(vertices, 12, 0),
                    new VertexBufferBinding(verticesColors, 16, 0)
                }

                );
            context.InputAssembler.SetIndexBuffer(triangles, Format.R16_UInt, 0);
            context.VertexShader.Set(vertexShader);
            context.Rasterizer.SetViewports(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
            context.PixelShader.Set(pixelShader);
            context.OutputMerger.SetTargets(renderView);




            //In HLSL global variables are considered uniform by default.

            //It's also settled that a variable coming out of the vertex shader stage for example is varying (HLSL doesn't need this keyword at all!).

            //Note that GLSL keywords uniform/varying are inherited from RSL (RenderMan shading language).


            //Uniform Parameter 
            // UniformHandle testFarbeHandle = pixelShader.GetUniformParam("TestFarbe");
            // pixelShader.SetUniform(testFarbeHandle, Handle.Color.Black);

            //Effect handle = new Effect(device, pixelShaderByteCode, EffectFlags.None);
            //handle.GetVariableByName("TestFarbe").AsVector().Set(Color.Red);

            //var getUniform = new Buffer(device, new BufferDescription
            //{
            //    Usage = ResourceUsage.Default,
            //    SizeInBytes = sizeof(Variables),
            //    BindFlags = BindFlags.ConstantBuffer,
            //    CpuAccessFlags=0
            //});

            //var cb = new Variables();
            //cb.R = 1.0f;
            //cb.G = 0.0f;
            //cb.B = 0.0f;
            //cb.A = 1.0f;


            //var data = new DataStream(sizeof(Variables), true, true);
            //data.Write(cb);
            //data.Position = 0;
            //context.UpdateSubresource(new DataBox(data.PositionPointer, 0, 0), getUniform, 0);
            //context.PixelShader.SetConstantBuffer(0, getUniform);


            //var shaderReflection = new ShaderReflection(pixelShaderByteCode);
            //var shaderDesc = shaderReflection.GetResourceBindingDescription(0);


            pRefelector = new ShaderReflection(pixelShaderByteCode);
            sDesc = pRefelector.Description;

            List<SharpDxShaderParamInfo> _sDxShaderParams = new List<SharpDxShaderParamInfo>();
            Dictionary<string, Buffer> _sDxShaderBuffers = new Dictionary<string, Buffer>();

            for (int i = 0; i < sDesc.ConstantBuffers; ++i)
            {
                ShaderParameterDescription paramDesc = pRefelector.GetInputParameterDescription(i);
                ConstantBuffer cbTemp = pRefelector.GetConstantBuffer(i);
                ConstantBufferDescription cbDEsc = cbTemp.Description;
                //cbDEsc.Name Liefert den Namen der Struktur
                if (_sDxShaderBuffers.ContainsKey(cbDEsc.Name))
                {
                    _sDxShaderBuffers.TryGetValue(cbDEsc.Name, out _buff);
                    _sDxShaderBuffers.Add(cbDEsc.Name, _buff);
                    SharpDxShaderParamInfo _params = new SharpDxShaderParamInfo();
                    
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        _params._ps = pixelShader;
                        _params._vs = vertexShader;
                        _params._psByteCode = pixelShaderByteCode;
                        _params._vsByteCode = vertexShaderByteCode;
                        _params._buffername = cbDEsc.Name;
                        _params._sdxBuffer = _buff;
                        _params._flags = shaderRefVar.Description.Flags;
                        _params._varCount = cbDEsc.VariableCount;
                        _params._varName = shaderRefVar.Description.Name;
                        _params._varSize = shaderRefVar.Description.Size;
                        _params._varPositionB = shaderRefVar.Description.StartOffset;
                        _params._flags = shaderRefVar.Description.Flags;
                        _params._varPositionI = j;
                        _params._varType = shaderRefType.Description.Type;
                        _params._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                        _sDxShaderParams.Add(_params);
                    }
                }
                else
                {
                    var bufferSize = cbDEsc.Size;
                    var _buffer = new Buffer(device, new BufferDescription
                        {
                            Usage = ResourceUsage.Default,
                            SizeInBytes = bufferSize,
                            BindFlags = BindFlags.ConstantBuffer,
                            CpuAccessFlags = 0
                        });
                    _sDxShaderBuffers.Add(cbDEsc.Name, _buffer);

                    SharpDxShaderParamInfo _params = new SharpDxShaderParamInfo();
                    _params._bufferParams = new DataStream(cbDEsc.Size, true, true);
                    for (int j = 0; j < cbDEsc.VariableCount; j++)
                    {
                        ShaderReflectionVariable shaderRefVar = cbTemp.GetVariable(j);
                        ShaderReflectionType shaderRefType = cbTemp.GetVariable(j).GetVariableType();
                        ShaderTypeDescription shaderRefTypeDesc = shaderRefType.Description;
                        _params._ps = pixelShader;
                        _params._vs = vertexShader;
                        _params._psByteCode = pixelShaderByteCode;
                        _params._vsByteCode = vertexShaderByteCode;
                        _params._buffername = cbDEsc.Name;
                        _params._sdxBuffer = _buffer;
                        _params._flags = shaderRefVar.Description.Flags;
                        _params._varCount = cbDEsc.VariableCount;
                        _params._varName = shaderRefVar.Description.Name;
                        _params._varSize = shaderRefVar.Description.Size;
                        _params._varPositionB = shaderRefVar.Description.StartOffset;
                        _params._flags = shaderRefVar.Description.Flags;
                        _params._varPositionI = j;
                        _params._varType = shaderRefType.Description.Type;
                        
                        
                        _params._bufferParams.Write(shaderRefVar.Description.DefaultValue);
                       
                        _sDxShaderParams.Add(_params);
                    }
                }
                
                
                
            }

            float R;
            float G;
            float B;
            float A;
            
            //foreach (var daten in _sDxShaderParams)
            //{
            //    if (daten._varSize == 16)
            //    {
                   
            //        R = 1.0f;
            //        G = 1.0f;
            //        B = 0.0f;
            //        A = 1.0f;
            //        var data = new DataStream(16, true, true);
            //        data.Write(R);
            //        data.Write(G);
            //        data.Write(B);
            //        data.Write(A);
            //        data.Position = daten._varPositionB;
            //        context.UpdateSubresource(new DataBox(data.PositionPointer, 0, 0), daten._sdxBuffer, 0);
            //        context.PixelShader.SetConstantBuffer(0, daten._sdxBuffer);
            //    }
            //    else
            //    {
            //        MessageBox.Show("Falsch");
            //    }
            //}
            int temp = 0;
            for (int i = 0; i < _sDxShaderParams.Count; i++)
            {
                
                if (_sDxShaderParams[i]._flags == ShaderVariableFlags.Used)
                {
                    R = 1.0f;
                    G = 0.0f;
                    B = 0.0f;
                    A = 1.0f;
                    _sDxShaderParams[i]._bufferParams.Position = (_sDxShaderParams[i]._varPositionB);
                    _sDxShaderParams[i]._bufferParams.Write(R);
                    _sDxShaderParams[i]._bufferParams.Write(G);
                    _sDxShaderParams[i]._bufferParams.Write(B);
                    _sDxShaderParams[i]._bufferParams.Write(A);
                    _sDxShaderParams[i]._bufferParams.Position = 0;
                    context.UpdateSubresource(new DataBox(_sDxShaderParams[i]._bufferParams.PositionPointer, 0, 0),
                                              _sDxShaderParams[i]._sdxBuffer, 0);
                    context.PixelShader.SetConstantBuffer(0, _sDxShaderParams[i]._sdxBuffer);
                }
                else
                {
                    
                }
                
            }




                // Main loop
                RenderLoop.Run(form, () =>
                    {
                        context.ClearRenderTargetView(renderView, Color.Black);

                        context.DrawIndexed(3, 0, 0);
                        swapChain.Present(0, PresentFlags.None);
                    });

            // Release all resources
            vertexShaderByteCode.Dispose();
            vertexShader.Dispose();
            pixelShaderByteCode.Dispose();
            pixelShader.Dispose();
            vertices.Dispose();
            layout.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();
            context.ClearState();
            context.Flush();
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }
    }


}