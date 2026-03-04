<script setup lang="ts">
import ControlButton from '@/components/UI/ControlButton.vue'

defineProps<{
  show: boolean
  title: string
}>()

const emit = defineEmits<{
  confirm: []
  cancel: []
}>()
</script>
<template>
  <Teleport to="body">
    <div v-if="show" class="modal-overlay" @click.self="emit('cancel')">
      <div class="modal">
        <h3 class="modal-title">{{ title }}</h3>

        <p class="modal-message">Are you sure?</p>

        <div class="modal-actions">
          <ControlButton @click="emit('cancel')" label="Cancel" />
          <ControlButton @click="emit('confirm')" label="Delete" type="danger" />
        </div>
      </div>
    </div>
  </Teleport>
</template>
<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  width: 320px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}

.modal-title {
  margin: 0;
}

.modal-message {
  color: var(--color-text-muted);
  margin-bottom: 1.5rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
}
</style>
