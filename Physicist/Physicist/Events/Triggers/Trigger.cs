﻿namespace Physicist.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using FarseerPhysics.Dynamics;
    using Microsoft.Xna.Framework;
    using Physicist.Actors;
    using Physicist.Controls;
    using Physicist.Enums;
    using Physicist.Extensions;

    public abstract class Trigger : PhysicistGameScreenItem, ITrigger
    {
        private bool isActive;
        private bool isEnabled;
        private bool canSwitch = true;
        private Dictionary<TriggerMode, Dictionary<string, IModifier>> modifiers = new Dictionary<TriggerMode, Dictionary<string, IModifier>>();

        protected Trigger()
        {
            this.IsReusable = true;
            this.IsEnabled = true;
            this.IsActive = false;
            this.IsInitialized = false;
            foreach (TriggerMode mode in Enum.GetValues(typeof(TriggerMode)))
            {
                this.modifiers.Add(mode, new Dictionary<string, IModifier>());
            }
        }

        public TriggerStyle Style { get; private set; }

        public bool IsInitialized { get; private set; }

        public bool IsReusable { get; private set; }

        public bool IsEnabled 
        {
            get
            {
                return this.IsInitialized && this.isEnabled;
            }

            set
            {
                this.isEnabled = value;
            }
        }

        public bool IsActive 
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isEnabled && this.isActive != value)
                {
                    this.modifiers.Values.ForEach(modifierlist => modifierlist.Values.ForEach(modifier => 
                    {
                        if (modifier != null) 
                        {
                            modifier.IsActive = !modifier.IsActive;
                        }
                    }));

                    this.canSwitch = true;
                }

                this.isActive = value;

                if (!this.IsReusable)
                {
                    this.IsEnabled = false;
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (this.IsEnabled)
            {
                if (this.IsActive)
                {
                    this.modifiers[TriggerMode.WhileActivated].Values.ForEach(modifier => 
                    { 
                        if (modifier != null) 
                        { 
                            modifier.Update(gameTime); 
                        } 
                    });
                }
                else
                {
                    this.modifiers[TriggerMode.WhileDeactivated].Values.ForEach(modifier => 
                    { 
                        if (modifier != null) 
                        { 
                            modifier.Update(gameTime); 
                        } 
                    });
                }
            }
        }

        public void Initialize(IEnumerable<IModifier> availableModifiers)
        {
            if (availableModifiers != null)
            {
                foreach (var modifier in availableModifiers)
                {
                    foreach (var mode in this.modifiers.Keys)
                    {
                        if (this.modifiers[mode].ContainsKey(modifier.Name))
                        {
                            this.modifiers[mode][modifier.Name] = modifier;
                            modifier.IsActive = mode == TriggerMode.OnDeactivated || mode == TriggerMode.WhileDeactivated;
                        }
                    }
                }

                this.IsInitialized = true;
            }
        }

        public override XElement XmlSerialize()
        {
            throw new NotImplementedException();
        }

        public override void XmlDeserialize(XElement element)
        {
            if (element != null)
            {
                var enabledAtt = element.Attribute("isEnabled");
                if (enabledAtt != null)
                {
                    this.IsEnabled = bool.Parse(enabledAtt.Value);
                }

                var reuseableAtt = element.Attribute("isReuseable");
                if (reuseableAtt != null)
                {
                    this.IsReusable = bool.Parse(reuseableAtt.Value);
                }

                var styleAtt = element.Attribute("style");
                if (styleAtt != null)
                {
                    this.Style = (TriggerStyle)Enum.Parse(typeof(TriggerStyle), styleAtt.Value);
                }

                foreach (TriggerMode mode in Enum.GetValues(typeof(TriggerMode)))
                {
                    var modeEle = element.Element(mode.ToString());
                    if (modeEle != null)
                    {
                        foreach (var modifierEle in modeEle.Elements("Modifier"))
                        {
                            this.modifiers[mode].Add(modifierEle.Attribute("name").Value, null);
                        }
                    }
                }
            }
        }

        protected void ActivateWithStyle()
        {
            if (this.Style == TriggerStyle.Button)
            {
                this.IsActive = true;
            }
            else if (this.Style == TriggerStyle.ToggleOnActivated)
            {
                this.IsActive = !this.IsActive;
            }
            else if (this.Style == TriggerStyle.DropLatch && this.canSwitch)
            {
                this.IsActive = true;
                this.canSwitch = false;
            }
        }

        protected void DeactivateWithStyle()
        {
            if (this.Style == TriggerStyle.Button)
            {
                this.IsActive = false;
            }
            else if (this.Style == TriggerStyle.ToggleOnDeactivated)
            {
                this.IsActive = !this.IsActive;
            }
            else if (this.Style == TriggerStyle.DropLatch && this.canSwitch)
            {
                this.IsActive = false;
                this.canSwitch = false;
            }
        }
    }
}
