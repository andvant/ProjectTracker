<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useIssuesStore } from '@/stores/issuesStore'
import { type IssueDto, IssueStatus, AddCommentRequest } from '@/types/issues'
import type { UsersDto } from '@/types/users'
import { ApiError, type ValidationErrors } from '@/types/api'
import { applyErrorsFromApi, createDefaultErrors, formatDate, getEnumOptions } from '@/utils'
import InputErrors from '@/components/UI/InputErrors.vue'
import ControlButton from '@/components/UI/ControlButton.vue'

const props = defineProps<{
  projectId: string
  issueId: string
  memberUsers: UsersDto[]
}>()

const issue = defineModel<IssueDto>()

const issuesStore = useIssuesStore()

const req = reactive(new AddCommentRequest())
const isAddingComment = ref(false)
const isSubmitting = ref(false)

type Errors = ValidationErrors<AddCommentRequest>

const errors = ref<Errors>(createDefaultErrors(req))

const validate = () => {
  errors.value = createDefaultErrors(req)

  let isValid = true

  if (!req.text.trim()) {
    errors.value.text = 'Comment cannot be empty'
    isValid = false
  }

  return isValid
}

const onAddingComment = () => {
  if (isAddingComment.value) return

  req.status = issue.value!.status
  req.assigneeId = issue.value!.assignee?.id
  isAddingComment.value = true
}

const onCancelComment = () => {
  errors.value = createDefaultErrors(req)
  req.text = ''
  isAddingComment.value = false
}

const onAddComment = async () => {
  if (!validate()) return

  try {
    isSubmitting.value = true

    issue.value = await issuesStore.addComment(props.projectId, props.issueId, req)

    onCancelComment()
  } catch (e) {
    if (e instanceof ApiError && e.problem) {
      applyErrorsFromApi(errors.value, e.problem)
    } else {
      errors.value.general = 'Unexpected error occurred'
    }
  } finally {
    isSubmitting.value = false
  }
}
</script>
<template>
  <div>
    <textarea
      @focus="onAddingComment"
      placeholder="Add a comment..."
      class="comment-input"
      :class="{ 'comment-input_expanded': isAddingComment }"
      v-model="req.text"
    ></textarea>
    <InputErrors :error="errors.text" />
    <InputErrors :error="errors.general" />
  </div>

  <div v-if="isAddingComment">
    <ControlButton @click="onAddComment" :disabled="isSubmitting" label="Save" type="primary" />
    <ControlButton @click="onCancelComment" label="Cancel" />

    <select v-model="req.status">
      <option
        v-for="status in getEnumOptions(IssueStatus)"
        :key="status.value"
        :value="status.value"
      >
        {{ status.label }}
      </option>
    </select>

    <select v-model="req.assigneeId">
      <option :value="undefined">Unassigned</option>
      <option v-for="user in memberUsers" :key="user.id" :value="user.id">
        {{ user.fullName }}
      </option>
    </select>
  </div>

  <div class="comments-wrapper">
    <div v-for="comment in issue!.comments" :key="comment.id">
      <div class="comment-name">{{ comment.user.fullName }}</div>
      <div class="comment-date">{{ formatDate(comment.createdAt) }}</div>
      <div class="comment-text">{{ comment.text }}</div>
    </div>
  </div>
</template>
<style scoped>
.comment-input {
  width: 85%;
  height: 2rem;
}

.comment-input_expanded {
  height: 10rem;
}

.comments-wrapper {
  width: 85%;
  display: flex;
  flex-direction: column;
  gap: 1.3rem;
}

.comment-name {
  font-weight: 600;
}

.comment-date {
  color: var(--color-text-muted);
  font-size: 0.9rem;
}

.comment-text {
  margin-top: 0.5rem;
}
</style>
