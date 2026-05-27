using System;
using System.Threading.Tasks;
using UnityEngine;
using GameCreator.Runtime.VisualScripting;
using GameCreator.Runtime.Variables;

namespace GameCreator.Runtime.Common
{
    [Version(1,0,0)]
    [Title("Delete Save Record")]
    [Description("Deletes a saved data record (slot+ID) from GC2's save storage. Works by SaveID or by reference to a save-aware variable. Returns whether deletion succeeded.")]
    [Category("Settings/Delete Save Record")]
    [Keywords("delete", "save", "playerprefs", "remove", "saveid")]
    [Image(typeof(IconDiskSolid), ColorTheme.Type.TextNormal)]
    public class InstructionDeleteSaveRecord : Instruction
    {
        public enum Method { ByReference, ByID, DeleteAll }

        [SerializeField] private Method m_Method = Method.ByID;
        [SerializeField] private PropertyGetString m_SaveID = new PropertyGetString("");
        [SerializeField] private PropertyGetInteger m_SaveSlot = new PropertyGetInteger(1);
        [SerializeField] private LocalNameVariables m_LocalNameVariables;
        [SerializeField] private GlobalNameVariables m_GlobalNameVariables;

        // Optional boolean result storage
        [SerializeField] private PropertySetBool m_StoreResult = SetBoolNone.Create;

        public override string Title
        {
            get
            {
                return m_Method switch
                {
                    Method.ByID => $"Delete ID:{m_SaveID} (slot {m_SaveSlot})",
                    Method.ByReference => $"Delete save for referenced variable (slot {m_SaveSlot})",
                    Method.DeleteAll => $"Delete ALL saved data (slot {m_SaveSlot})",
                    _ => string.Empty
                };
            }
        }

        protected override async Task Run(Args args)
        {
            SaveLoadManager manager = SaveLoadManager.Instance;
            if (manager == null)
            {
                Debug.LogError("[InstructionDeleteSaveRecord] SaveLoadManager.Instance is null");
                m_StoreResult?.Set(false, args);
                return;
            }

            int slot = (int)m_SaveSlot.Get(args);
            string saveID = null;

            if (m_Method == Method.DeleteAll)
            {
                try
                {
                    await manager.DataStorage.DeleteAll();
                    Debug.Log($"[InstructionDeleteSaveRecord] Deleted ALL save data for storage at slot {slot}.");
                    m_StoreResult?.Set(true, args);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[InstructionDeleteSaveRecord] DeleteAll failed: {e.Message}");
                    m_StoreResult?.Set(false, args);
                }

                return;
            }

            if (m_Method == Method.ByID)
            {
                saveID = m_SaveID.Get(args);
                if (string.IsNullOrEmpty(saveID))
                {
                    Debug.LogError("[InstructionDeleteSaveRecord] SaveID is empty for ByID method.");
                    m_StoreResult?.Set(false, args);
                    return;
                }
            }
            else
            {
                // Try to obtain save id from a referenced variable (local or global name var manager)
                if (m_LocalNameVariables != null) saveID = m_LocalNameVariables.SaveID;
                else if (m_GlobalNameVariables != null) saveID = GlobalNameVariablesManager.Instance?.SaveID;

                if (string.IsNullOrEmpty(saveID))
                {
                    Debug.LogError("[InstructionDeleteSaveRecord] No referenced save variable found (check Local or Global reference)." );
                    m_StoreResult?.Set(false, args);
                    return;
                }
            }

            // Build the storage key used by SaveLoadManager
            string dbKey = string.Format("data-{0:D4}-{1}", slot, saveID);

            try
            {
                // Check whether key exists (Get returns null if absent for most storages)
                var existing = await manager.DataStorage.Get(dbKey, typeof(object));
                if (existing == null)
                {
                    Debug.LogWarning($"[InstructionDeleteSaveRecord] No save record found for '{dbKey}' to delete.");
                    m_StoreResult?.Set(false, args);
                    return;
                }

                await manager.DataStorage.DeleteKey(dbKey);
                Debug.Log($"[InstructionDeleteSaveRecord] Deleted save key '{dbKey}'");
                m_StoreResult?.Set(true, args);
                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"[InstructionDeleteSaveRecord] Failed to delete key '{dbKey}': {e.Message}");
                m_StoreResult?.Set(false, args);
                return;
            }
        }
    }
}
